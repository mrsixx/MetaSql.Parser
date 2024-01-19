using System;

namespace MetaSql.Parser.Exceptions
{
    public class UnrecognizedTypeException : Exception
    {
        public UnrecognizedTypeException(string filterName) : base($"Filtro {filterName} tipo de valor default não reconhecido.") { }
    }
}
