using System;

namespace MetaSql.Parser.Interfaces
{
    internal interface IValueType<T> : IEquatable<T>
    {
        T Value { get; }
    }
}
