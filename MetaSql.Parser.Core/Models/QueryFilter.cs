using System;
using static MetaSql.Parser.Enums.FilterTypeEnum;

namespace MetaSql.Parser.Models
{
    public class QueryFilter
    {
        public string Name { get; internal set; }

        public FilterType Type { get; internal set; }


        public string Alias { get; internal set; }

        public FilterValue DefaultValue { get; internal set; }

        public string Text { get; internal set; }

        public bool IsDetail { get; internal set; }

        public bool Hidden { get; internal set; }

        public string LookupSource { get; internal set; }

        public bool HasAlias => Alias != default;

        public bool HasDefaultValue => DefaultValue != null;

        public bool HasLookup => !String.IsNullOrWhiteSpace(LookupSource);

        public bool Block { get; internal set; }

    }
}
