#pragma warning disable CA1032 // Implement standard exception constructors
using System;

namespace Lox.Runtime
{
    class LoxBreakException : Exception { }
}
#pragma warning restore CA1032 // Implement standard exception constructors