using MetaSql.Parser.Interfaces;
using System;

namespace MetaSql.Parser.Models
{
    public class FilterDatetimeRangeValue : FilterValue, IValueType<Tuple<FilterValue, FilterValue>>
    {
        public FilterDatetimeRangeValue(FilterValue start, FilterValue end)
        {
            Value = new Tuple<FilterValue, FilterValue>(start, end);
        }

        public Tuple<FilterValue, FilterValue> Value { get; internal set; }

        public bool Equals(Tuple<FilterValue, FilterValue> other)
        {
            bool isStartEqual = CompareValues(Value.Item1, other.Item1);
            var isEndEqual = CompareValues(Value.Item2, other.Item2);

            return isStartEqual && isEndEqual;
        }

        private bool CompareValues(FilterValue thisValue, FilterValue otherValue)
        {
            var isStartEqual = false;

            if (otherValue is FilterDateTimeValue otherDt1 && thisValue is FilterDateTimeValue thisDt1)
                isStartEqual = thisDt1.Equals(otherDt1.Value);
            else if (otherValue is FilterFunctionValue otherFunc1 && thisValue is FilterFunctionValue thisFunc1)
                isStartEqual = thisFunc1.Equals(otherFunc1.Value);
            return isStartEqual;
        }
    }
}
