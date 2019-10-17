using System;

namespace LoxFramework
{
    public class InterpreterEventArgs : EventArgs
    {
        public InterpreterEventArgs(string message)
        {
            Message = message;
        }
        
        public string Message
        {
            private set;
            get;
        }
    }
}
