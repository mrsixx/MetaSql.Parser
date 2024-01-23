using MetaSql.Parser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetaSql.Parser.Models
{
    public class FilterTextValue : FilterValue, ISqlConstant, IValueType<string>
    {
        public FilterTextValue(string strValue)
        {
            Value = strValue;
        }

        public string Value { get; internal set; }

        public string SqlFormattedValue => $"'{Value}'";

        public bool Equals(string other) => Value.Equals(other);
        
        public object GetValue() => Value;
    }
}
