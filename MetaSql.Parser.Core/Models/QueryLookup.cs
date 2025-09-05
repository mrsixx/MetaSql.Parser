using System;
using System.Collections.Generic;
using System.Text;

namespace MetaSql.Parser.Models
{
    public class QueryLookup
    {
        public QueryLookup(string id, string query, string fullClause)
        {
            Id = id;
            Query = query;
            FullClause = fullClause;
        }

        public string Id { get; internal set; }

        public string Query { get; internal set; }

        public string FullClause { get; internal set; }
    }
}
