#pragma warning disable CA1032 // Implement standard exception constructors
using Scanning;
using System;

namespace Lox.Runtime
{
    class LoxRunTimeException : Exception
    {
        public Token Token { get; private set; }

        public LoxRunTimeException(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}
#pragma warning restore CA1032 // Implement standard exception constructors