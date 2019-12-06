using System;
using System.Runtime.Serialization;

namespace LoxFramework.Evaluating
{
    [Serializable]
    internal class LoxBreakException : Exception
    {
        public LoxBreakException() { }

        public LoxBreakException(string message) : base(message) { }

        public LoxBreakException(string message, Exception innerException) : base(message, innerException) { }

        protected LoxBreakException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}