#pragma warning disable CA1032 // Implement standard exception constructors
using System;

namespace LoxVM
{
    [Serializable]
    internal class CompileException : Exception
    {
        public CompileException() { }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors