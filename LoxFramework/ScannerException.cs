using System;

namespace LoxFramework
{
    class ScannerException : Exception
    {
        public int Line
        {
            private set;
            get;
        }

        public ScannerException(int line, string message) : base(message)
        {
            Line = line;
        }
    }
}
