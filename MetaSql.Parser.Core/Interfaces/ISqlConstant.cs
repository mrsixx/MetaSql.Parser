namespace MetaSql.Parser.Interfaces
{
    public interface ISqlConstant
    {
        string SqlFormattedValue { get; }

        object GetValue();
    }
}
