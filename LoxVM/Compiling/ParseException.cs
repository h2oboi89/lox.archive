#pragma warning disable CA1032 // Implement standard exception constructors
using System;

namespace LoxVM.Compiling
{
    class ParseException : Exception
    {
        public ParseException() { }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructorsusing System;
