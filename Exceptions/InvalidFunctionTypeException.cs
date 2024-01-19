using System;
using static MetaSql.Parser.Enums.FilterTypeEnum;

namespace MetaSql.Parser.Exceptions
{
    public class InvalidFunctionTypeException : Exception
    {
        public InvalidFunctionTypeException(string name, FilterType type) : base($"Função {name} deve ser atribuída a um filtro de tipo ${type}") { }
    }
}
