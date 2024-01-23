namespace MetaSql.Parser.Models
{
    public class QueryTable
    {
        public QueryTable(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }

        public int Ocurrencies { get; private set; }

        public void IncrementOcurrencies() => Ocurrencies++;
    }
}
