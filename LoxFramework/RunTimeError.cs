using LoxFramework.Scanning;
using System;
using System.Runtime.Serialization;

namespace LoxFramework
{
    [Serializable]
    internal class RunTimeError : Exception
    {
        public Token Token { get; private set; }

        public RunTimeError() { }

        public RunTimeError(string message) : base(message) { }

        public RunTimeError(Token token, string message) : base(message)
        {
            Token = token;
        }

        public RunTimeError(string message, Exception innerException) : base(message, innerException) { }

        protected RunTimeError(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}