using MetaSql.Parser.Interfaces;
using System;
using System.Globalization;

namespace MetaSql.Parser.Models
{
    public class FilterDecimalValue : FilterValue, ISqlConstant, IValueType<decimal>
    {
        public FilterDecimalValue(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; internal set; }

        public string SqlFormattedValue => Convert.ToString(Value, CultureInfo.InvariantCulture);

        public bool Equals(decimal other) => Value.Equals(other);
    }
}
