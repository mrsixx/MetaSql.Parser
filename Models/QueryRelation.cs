namespace MetaSql.Parser.Models
{
    public class QueryRelation
    {
        public QueryRelation(string leftTable, string rightTable)
        {
            LeftTable = leftTable;
            RightTable = rightTable;
        }

        public string LeftTable { get; set; }

        public string RightTable { get; set; }

    }
}
