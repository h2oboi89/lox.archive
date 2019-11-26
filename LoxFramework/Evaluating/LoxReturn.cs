using System;
using System.Runtime.Serialization;

namespace LoxFramework.Evaluating
{
    class LoxReturn : Exception
    {
        public object Value { get; private set; }

        public LoxReturn() { }

        public LoxReturn(object value)
        {
            Value = value;
        }

        public LoxReturn(string message) : base(message) { }

        public LoxReturn(string message, Exception innerException) : base(message, innerException) { }

        protected LoxReturn(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
