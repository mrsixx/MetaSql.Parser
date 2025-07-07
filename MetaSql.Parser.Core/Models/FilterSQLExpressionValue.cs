using MetaSql.Parser.Interfaces;

namespace MetaSql.Parser.Models
{
    public class FilterSQLExpressionValue : FilterValue, IValueType<string>
    {
        public FilterSQLExpressionValue(string expression)
        {
            Value = expression;
        }

        public bool Equals(string other) => Value.Equals(other);

        public string Value { get; }
    }
}
