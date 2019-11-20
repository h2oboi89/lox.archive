using LoxFramework.Scanning;
using System;
using System.Runtime.Serialization;

namespace LoxFramework.Evaluating
{
    [Serializable]
    internal class LoxRunTimeException : Exception
    {
        public Token Token { get; private set; }

        public LoxRunTimeException() { }

        public LoxRunTimeException(string message) : base(message) { }

        public LoxRunTimeException(Token token, string message) : base(message)
        {
            Token = token;
        }

        public LoxRunTimeException(string message, Exception innerException) : base(message, innerException) { }

        protected LoxRunTimeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}