using System;

namespace LoxFramework
{
    /// <summary>
    /// Exception thrown when <see cref="Scanner"/> encounters an error during scanning.
    /// </summary>
    class ScannerException : Exception
    {
        /// <summary>
        /// Line the error was encountered on
        /// </summary>
        public int Line
        {
            private set;
            get;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="line">Line the error was encountered on.</param>
        /// <param name="message">Details about the error.</param>
        public ScannerException(int line, string message) : base(message)
        {
            Line = line;
        }
    }
}
