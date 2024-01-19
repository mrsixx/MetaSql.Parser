using MetaSql.Parser.Models;
using static MetaSql.Parser.TSqlParser;

namespace MetaSql.Parser.Interfaces
{
    internal interface IFilterValueFactory
    {
        FilterValue GetValue(Filter filter, Efilter_default_expressionContext defaultExpressionCtx);
    }
}
