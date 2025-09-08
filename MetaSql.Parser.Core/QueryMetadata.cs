
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.Text;
using MetaSql.Parser.Models;
using System.Text.RegularExpressions;

namespace MetaSql.Parser
{
    public class QueryMetadata
    {
        public string ResultQuery { get; set; }

        public List<QueryTable> Tables { get; }

        public List<QueryRelation> Relations { get; }

        public List<QueryFilter> Filters { get; }

        public Dictionary<string, QueryLookup> Lookups { get; }

        public List<QueryExecute> Executes { get; }


        /// <summary>
        /// Retorna a query com os blocos de filtros desejados.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public string GetResultQueryWithBlocks(params string[] names)
        {
            string pattern = @"<(?<tag>\w+)>(?<content>.*?)</\k<tag>>";
            var regex = new Regex(pattern, RegexOptions.Singleline);

            // Substitui cada bloco conforme a lógica
            return regex.Replace(ResultQuery, match =>
            {
                string tagName = match.Groups["tag"].Value;
                string content = match.Groups["content"].Value;

                // Se o nome do filtro estiver na lista, mantém o conteúdo e remove as tags
                if (Array.Exists(names, name => name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
                {
                    return content;
                }

                // Caso contrário, remove o bloco inteiro
                return string.Empty;
            });
        }



        public QueryMetadata()
        {
            Tables = new List<QueryTable>();
            Relations = new List<QueryRelation>();
            Filters = new List<QueryFilter>();
            Executes = new List<QueryExecute>();
            Lookups = new Dictionary<string, QueryLookup>();
        }
    }
}
