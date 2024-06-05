using MetaSql.Parser.Enums;
using MetaSql.Parser.Interfaces;

namespace MetaSql.Parser.Factories
{
    internal class FilterTypeFactory : IFilterTypeFactory
    {
        public FilterTypeEnum.FilterType GetType(string typeName)
        {
            switch (typeName.ToUpperInvariant())   
            {
                case "INTEGER": return FilterTypeEnum.FilterType.Integer;
                case "DECIMAL": return FilterTypeEnum.FilterType.Decimal;
                case "DATE": return FilterTypeEnum.FilterType.Date;
                case "DATETIME": return FilterTypeEnum.FilterType.DateTime;
                case "TEXT": return FilterTypeEnum.FilterType.Text;
                case "DATETIMERANGE": return FilterTypeEnum.FilterType.DateTimeRange;
                default: return FilterTypeEnum.FilterType.Undefined;
            }
        }
    }
}
