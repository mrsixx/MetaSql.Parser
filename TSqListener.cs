using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using MetaSql.Parser.Exceptions;
using MetaSql.Parser.Factories;
using MetaSql.Parser.Interfaces;
using MetaSql.Parser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static MetaSql.Parser.TSqlParser;

namespace MetaSql.Parser
{

    internal class TSqListener : TSqlParserBaseListener
    {
        public string OriginalQuery { get; }
        public Dictionary<string, string> TableAliases { get; }

        public QueryMetadata Metadata { get; }

        private readonly IFilterTypeFactory _filterTypeFactory;
        private readonly IFilterValueFactory _filterValueFactory;

        public TSqListener(string query)
        {
            OriginalQuery = query;
            Metadata = new QueryMetadata();
            TableAliases = new Dictionary<string, string>();
            _filterTypeFactory = new FilterTypeFactory();
            _filterValueFactory = new FilterValueFactory();
        }

        public override void EnterTable_source_item([NotNull] TSqlParser.Table_source_itemContext ctx)
        {
            if (!ctx.IsEmpty && ctx.GetChild(0) is TSqlParser.Full_table_nameContext tableNameContext)
            {
                var currentTableName = tableNameContext.GetText().ToUpperInvariant();
                if (ctx.ChildCount > 1 && ctx.GetChild(1) is TSqlParser.As_table_aliasContext aliasContext)
                {
                    var alias = aliasContext.ChildCount > 1 ? aliasContext.GetChild(1).GetText().ToUpperInvariant() : aliasContext.GetText().ToUpperInvariant();
                    if (!TableAliases.ContainsKey(alias))
                        TableAliases.Add(alias, currentTableName);
                }
                else if (!TableAliases.ContainsKey(currentTableName))
                    TableAliases.Add(currentTableName, currentTableName);


                var table = Metadata.Tables.Find(t => t.TableName == currentTableName);
                if (table is null)
                {
                    table = new QueryTable(currentTableName);
                    Metadata.Tables.Add(table);
                }
                table.IncrementOcurrencies();
            }
        }

        public override void EnterPredicate([NotNull] TSqlParser.PredicateContext context)
        {
            if (context.ChildCount == 3)
            {
                var child1 = context.GetChild(0);
                var child2 = context.GetChild(1);
                var child3 = context.GetChild(2);
                if (child1 is TSqlParser.ExpressionContext expl &&
                   child2 is TSqlParser.Comparison_operatorContext op &&
                   child3 is TSqlParser.ExpressionContext expr)
                {
                    if (expl.GetChild(0) is TSqlParser.Full_column_nameContext leftColumn && expr.GetChild(0) is TSqlParser.Full_column_nameContext rigthColumn)
                    {
                        string leftColumnName = ExtractColumnName(leftColumn);
                        string rigthColumnName = ExtractColumnName(rigthColumn);
                        if (!String.IsNullOrWhiteSpace(leftColumnName) && !String.IsNullOrWhiteSpace(rigthColumnName))
                            Metadata.Relations.Add(new QueryRelation(leftColumnName, rigthColumnName));
                    }
                }
            }
        }

        public override void EnterSelect_statement([NotNull] TSqlParser.Select_statementContext context)
        {
            TableAliases.Clear();
        }

        public override void ExitSelect_statement([NotNull] TSqlParser.Select_statementContext context)
        {
            var query = OriginalQuery;

            Metadata.Filters.ForEach(f => query = Regex.Replace(query, $"{f.Text}(;)?", String.Empty));
            Metadata.ResultQuery = Regex.Replace(query, @"(\s){2,}|(\n)", " ", RegexOptions.IgnoreCase).Trim();

            var pattern = "&([a-zA-Z_$@#:0-9])+";
            Debug.WriteLine("Filters: ");
            foreach (var match in Regex.Matches(Metadata.ResultQuery, pattern))
            {
                var filter = Metadata.Filters.Find(f => $"&{f.Name}" == match.ToString());
                if (filter is null)
                    throw new UndeclaredFilterException(match.ToString());
                Metadata.ResultQuery = Regex.Replace(Metadata.ResultQuery, $"&{filter.Name}", $"@{filter.Name}");
                Debug.WriteLine($"{filter.Name}: {filter.Type}");
            }

            if (Metadata.Relations.Count <= 0)
                return;

            Debug.WriteLine("Aliases: ");
            foreach (var tableName in TableAliases.Keys)
                Debug.WriteLine($"{tableName} -> {TableAliases[tableName]}");

            Debug.WriteLine("Relations: ");
            foreach (var relation in Metadata.Relations)
                Debug.WriteLine($"{relation.LeftTable} <-> {relation.RightTable}");
        }

        public override void EnterEfilter_statement([NotNull] TSqlParser.Efilter_statementContext context)
        {
            if (!context.IsEmpty && context.GetChild(1) is TerminalNodeImpl filterName && context.GetChild(2) is Efilter_data_typeContext dataType)
            {
                var filter = new Filter
                {
                    Name = filterName.GetText().Replace("&", String.Empty),
                    Type = _filterTypeFactory.GetType(dataType.GetText()),
                    Text = context.Start.InputStream.GetText(new Interval(context.Start.StartIndex, context.Stop.StopIndex))
                };

                if (context.children.Any(c => c is As_column_aliasContext))
                {
                    var asColumnAliasCtx = context.GetRuleContext<As_column_aliasContext>(0);
                    filter.Alias = asColumnAliasCtx.GetChild<Column_aliasContext>(0).GetText().Replace("\'", String.Empty);
                }

                if (context.children.Any(c => c is Efilter_default_expressionContext))
                {
                    var defaultExpressionCtx = context.GetRuleContext<Efilter_default_expressionContext>(0);
                    filter.DefaultValue = _filterValueFactory.GetValue(filter, defaultExpressionCtx);
                }
                else if (context.children.Any(c => c is Efilter_detail_expressionContext))
                {
                    filter.IsDetail = true;
                }

                if (Metadata.Filters.Any(f => f.Name == filter.Name))
                    throw new InvalidOperationException($"{filter.Name} já está sendo utilizado como nome de outro filtro.");


                Metadata.Filters.Add(filter);

            }
        }

        /// <summary>
        /// Monta o nome completo da coluna NOMETABELA.NOMECOLUNA
        /// </summary>
        /// <param name="fullColumnNameCtx"></param>
        /// <returns></returns>
        private string ExtractColumnName(TSqlParser.Full_column_nameContext fullColumnNameCtx)
        {
            var strEmpty = "\"\"";
            var columnName = fullColumnNameCtx.GetText().ToUpperInvariant();

            if (columnName == strEmpty) return null;
            if (fullColumnNameCtx.ChildCount < 3) return null;

            if (fullColumnNameCtx.GetChild(0) is TSqlParser.Full_table_nameContext tblNameCtx && fullColumnNameCtx.GetChild(2) is TSqlParser.Id_Context colNameCtx)
            {
                var alias = tblNameCtx.GetText().ToUpperInvariant();
                if (!TableAliases.TryGetValue(alias, out string tblName)) return null;
                return $"{tblName}.{colNameCtx.GetText()}".ToUpperInvariant();
            }

            return null;
        }

    }
}
