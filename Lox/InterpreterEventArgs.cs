using System;

namespace Lox
{
    public class InterpreterEventArgs : EventArgs
    {
        /// <summary>
        /// Details about the event.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// True for non-print statements.
        /// </summary>
        public bool Optional { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Event message.</param>
        /// <param name="optional">True for output from statements other than print.</param>
        public InterpreterEventArgs(string message, bool optional = false)
        {
            Message = message;
            Optional = optional;
        }
    }
}
