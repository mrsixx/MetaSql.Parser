using Antlr4.Runtime.Tree;
using MetaSql.Parser.Enums;
using MetaSql.Parser.Exceptions;
using MetaSql.Parser.Interfaces;
using MetaSql.Parser.Models;
using System;
using System.Data;
using static MetaSql.Parser.Enums.DefaultValueFuncionEnum;
using static MetaSql.Parser.TSqlParser;

namespace MetaSql.Parser.Factories
{
    internal class FilterValueFactory : IFilterValueFactory
    {
        public FilterValue GetValue(QueryFilter filter, Efilter_default_expressionContext defaultExpressionCtx)
        {
            // range
            if(defaultExpressionCtx.GetChild(1) is Efilter_range_exprContext rangeExpression)
            {
                if (rangeExpression.GetChild(1) is Efilter_expression_function_or_constantContext startValueContext
                    && rangeExpression.GetChild(3) is Efilter_expression_function_or_constantContext endValueContext)
                {
                    // TODO: implementar diferentes tipos de intervalo (data, inteiros, etc)
                    var startValue = GetFilterFunctionOrConstantValue(startValueContext, FilterTypeEnum.FilterType.DateTime);
                    var endValue = GetFilterFunctionOrConstantValue(endValueContext, FilterTypeEnum.FilterType.DateTime);
                    if(filter.Type == FilterTypeEnum.FilterType.DateTimeRange) { 
                    
                        return new FilterDatetimeRangeValue(startValue, endValue);
                    }

                    throw new UnrecognizedTypeException(rangeExpression.GetText());

                }
                throw new ArgumentException("Os argumentos do intervalo não foram reconhecidos.");
            }

            // funções 
            if (defaultExpressionCtx.GetChild(1) is Efilter_default_expression_functionContext functionValue)
                return GetFilterFunctionValue(functionValue, filter.Type);
            
            // valores constantes
            if (defaultExpressionCtx.GetChild(1) is ConstantContext constant)
                return GetFilterConstantValue(constant, filter.Type);
            // expressions
            if (defaultExpressionCtx.GetChild(1) is ExpressionContext expression)
            {
                var sqlExpression = expression.GetChild(0).GetText();
                return new FilterSQLExpressionValue(sqlExpression);
            }

            throw new UnrecognizedTypeException(filter.Name);
        }
        private FilterValue GetFilterFunctionOrConstantValue(Efilter_expression_function_or_constantContext context, FilterTypeEnum.FilterType filterType)
        {
            if (context.GetChild(0) is Efilter_default_expression_functionContext functionValue)
                return GetFilterFunctionValue(functionValue, filterType);

            if(context.GetChild(0) is ConstantContext constant)
                return GetFilterConstantValue(constant, filterType);

            throw new UnrecognizedTypeException("Tipo do valor não reconhecido");
        }

        private FilterFunctionValue GetFilterFunctionValue(Efilter_default_expression_functionContext functionValue, FilterTypeEnum.FilterType filterType)
        {
            var strValue = functionValue.GetText();
            switch (strValue.ToUpperInvariant())
            {
                case "USUARIO()":
                    if (filterType != FilterTypeEnum.FilterType.Integer)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.Integer);
                    return new FilterFunctionValue(DefaultValueFunction.Usuario);
                case "DIAATUAL()":
                    if (filterType != FilterTypeEnum.FilterType.Integer)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.Integer);
                    return new FilterFunctionValue(DefaultValueFunction.DiaAtual);
                case "MESATUAL()":
                    if (filterType != FilterTypeEnum.FilterType.Integer)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.Integer);
                    return new FilterFunctionValue(DefaultValueFunction.MesAtual);
                case "ANOATUAL()":
                    if (filterType != FilterTypeEnum.FilterType.Integer)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.Integer);
                    return new FilterFunctionValue(DefaultValueFunction.AnoAtual);
                case "INICIO_MES_ATUAL()":
                    if (filterType != FilterTypeEnum.FilterType.Date && filterType != FilterTypeEnum.FilterType.DateTime)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.DateTime);
                    return new FilterFunctionValue(DefaultValueFunction.InicioMesAtual);
                case "FIM_MES_ATUAL()":
                    if (filterType != FilterTypeEnum.FilterType.Date && filterType != FilterTypeEnum.FilterType.DateTime)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.DateTime);
                    return new FilterFunctionValue(DefaultValueFunction.FimMesAtual);
                case "ONTEM()":
                    if (filterType != FilterTypeEnum.FilterType.Date && filterType != FilterTypeEnum.FilterType.DateTime)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.DateTime);
                    return new FilterFunctionValue(DefaultValueFunction.Ontem);
                case "HOJE()":
                    if (filterType != FilterTypeEnum.FilterType.Date && filterType != FilterTypeEnum.FilterType.DateTime)
                        throw new InvalidFunctionTypeException(strValue, FilterTypeEnum.FilterType.DateTime);
                    return new FilterFunctionValue(DefaultValueFunction.Hoje);
                default: throw new UnrecognizedTypeException(functionValue.GetText());
            }
        }

        private FilterValue GetFilterConstantValue(ConstantContext constantValue, FilterTypeEnum.FilterType filterType)
        {
            var strValue = constantValue.GetText();
            switch (filterType)
            {
                case FilterTypeEnum.FilterType.Integer:
                    if (Int32.TryParse(strValue, out int @int))
                        return new FilterIntegerValue(@int);
                    throw new MismatchedTypesException(strValue, typeof(int));
                case FilterTypeEnum.FilterType.Decimal:
                    if (Decimal.TryParse(strValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal @decimal))
                        return new FilterDecimalValue(@decimal);
                    throw new MismatchedTypesException(strValue, typeof(decimal));
                case FilterTypeEnum.FilterType.Date:
                    if (DateTime.TryParse(strValue.Replace("\'", String.Empty), out DateTime @date))
                        return new FilterDateValue(@date);
                    throw new MismatchedTypesException(strValue, typeof(DateTime));
                case FilterTypeEnum.FilterType.DateTime:
                    if (DateTime.TryParse(strValue.Replace("\'", String.Empty), out DateTime @datetime))
                        return new FilterDateTimeValue(@datetime);
                    throw new MismatchedTypesException(strValue, typeof(DateTime));
                case FilterTypeEnum.FilterType.Text:
                    return new FilterTextValue(strValue.Replace("\'", String.Empty));
                case FilterTypeEnum.FilterType.Undefined:
                default:
                    throw new NotImplementedException($"{filterType} não implementado.");
            }
        }
    }
}
