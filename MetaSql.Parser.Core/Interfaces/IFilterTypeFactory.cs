using static MetaSql.Parser.Enums.FilterTypeEnum;

namespace MetaSql.Parser.Interfaces
{
    public interface IFilterTypeFactory
    {
        FilterType GetType(string typeName);
    }
}
