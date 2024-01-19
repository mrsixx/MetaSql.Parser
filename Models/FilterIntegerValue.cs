using MetaSql.Parser.Interfaces;
using System;

namespace MetaSql.Parser.Models
{
    public class FilterIntegerValue : FilterValue, ISqlConstant, IValueType<int>
    {

        public FilterIntegerValue(int value)
        {
            Value = value;
        }
        
        public int Value { get; internal set; }

        public string SqlFormattedValue => Convert.ToString(Value);

        public bool Equals(int other) => Value.Equals(other);

    }
}
