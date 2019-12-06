#pragma warning disable CA1032 // Implement standard exception constructors
using System;

namespace Lox
{
    class LoxReturn : Exception
    {
        public object Value { get; private set; }

        public LoxReturn(object value)
        {
            Value = value;
        }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors
