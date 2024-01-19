using MetaSql.Parser.Interfaces;
using System;

namespace MetaSql.Parser.Models
{
    public class FilterDecimalValue : FilterValue, ISqlConstant, IValueType<decimal>
    {
        public FilterDecimalValue(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; internal set; }

        public string SqlFormattedValue => Convert.ToString(Value);

        public bool Equals(decimal other) => Value.Equals(other);
    }
}
