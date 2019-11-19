using System;

namespace LoxFramework
{
    /// <summary>
    /// Event arguments for Interpreter Events
    /// </summary>
    public class InterpreterEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Event message.</param>
        public InterpreterEventArgs(string message)
        {
            Message = message;
        }
        
        /// <summary>
        /// Details about the event.
        /// </summary>
        public string Message
        {
            private set;
            get;
        }
    }
}
