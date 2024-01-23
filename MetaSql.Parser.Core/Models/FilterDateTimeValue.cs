using MetaSql.Parser.Interfaces;
using System;

namespace MetaSql.Parser.Models
{
    public class FilterDateTimeValue : FilterValue, ISqlConstant, IValueType<DateTime>
    {
        public FilterDateTimeValue(DateTime value)
        {
            Value = value;
        }

        public DateTime Value { get; internal set; }

        public string SqlFormattedValue => $"'{Value:yyyy-MM-dd HH:mm:ss}'";

        public bool Equals(DateTime other) => Value.Equals(other);

        public object GetValue() => Value;
    }
}
