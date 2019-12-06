using System;
using System.Runtime.Serialization;

namespace LoxFramework.Evaluating
{
    [Serializable]
    internal class LoxContinueException : Exception
    {
        public LoxContinueException()
        {
        }

        public LoxContinueException(string message) : base(message)
        {
        }

        public LoxContinueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoxContinueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}