using System;

namespace MetaSql.Parser.Exceptions
{
    public class MismatchedTypesException : Exception
    {
        public MismatchedTypesException(string value, Type type) : base($"{value} não pode ser convertido para o tipo {type}") { }
    }
}
