
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using MetaSql.Parser.Models;

namespace MetaSql.Parser
{
    public class QueryMetadata
    {
        public string ResultQuery { get; set; }

        public List<QueryTable> Tables { get; }

        public List<QueryRelation> Relations { get; }

        public List<QueryFilter> Filters { get; }

        public QueryMetadata()
        {
            Tables = new List<QueryTable>();
            Relations = new List<QueryRelation>();
            Filters = new List<QueryFilter>();
        }

        public void CopyQuery(string realQuery)
        {

        }
    }
}
