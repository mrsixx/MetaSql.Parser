using MetaSql.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetaSql.Parser.Models
{
    public class FilterDateValue : FilterValue, ISqlConstant, IValueType<DateTime>
    {
        public FilterDateValue(DateTime value)
        {
            Value = value.Date;
        }

        public DateTime Value { get; internal set; }

        public string SqlFormattedValue => $"'{Value:yyyy-MM-dd}'";

        public bool Equals(DateTime other) => Value.Equals(other);
        
        public object GetValue() => Value;

    }
}
