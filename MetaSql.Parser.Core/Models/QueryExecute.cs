using System.Collections.Generic;

namespace MetaSql.Parser.Models
{
    public class QueryExecute
    {
        public QueryExecute()
        {
            Arguments = new List<string>();
        }

        public string Name { get; set; }

        public List<string> Arguments { get; }

        public string Text { get; internal set; }
    }
}
