using System;

namespace MetaSql.Parser.Exceptions
{
    public class UndeclaredFilterException : Exception
    {
        public UndeclaredFilterException(string filterName) : base($"Filtro {filterName} não foi declarado.") { }
    }
}
