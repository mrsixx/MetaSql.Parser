using static MetaSql.Parser.Enums.FilterTypeEnum;

namespace MetaSql.Parser.Models
{
    public class Filter
    {
        public string Name { get; internal set; }

        public FilterType Type { get; internal set; }

        public string Alias { get; internal set; }

        public FilterValue DefaultValue { get; internal set; }

        public string Text { get; internal set; }

        public bool IsDetail { get; internal set; }

        public bool HasAlias => Alias != default;

        public bool HasDefaultValue => DefaultValue != null;

    }
}
