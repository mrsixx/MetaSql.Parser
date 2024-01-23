using MetaSql.Parser.Interfaces;
using static MetaSql.Parser.Enums.DefaultValueFuncionEnum;

namespace MetaSql.Parser.Models
{
    public class FilterFunctionValue : FilterValue, IValueType<DefaultValueFunction>
    {
        public FilterFunctionValue(DefaultValueFunction value)
        {
            Value = value;
        }

        public DefaultValueFunction Value { get; internal set; }

        public bool Equals(DefaultValueFunction other) => Value == other;

    }
}