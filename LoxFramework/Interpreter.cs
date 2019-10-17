using System;

namespace LoxFramework
{
    public static class Interpreter
    {
        public static void Run(string source)
        {
            try
            {
                var tokens = Scanner.Scan(source);

                foreach (var token in tokens)
                {
                    Status?.Invoke(null, new InterpreterEventArgs(token.ToString()));
                }
            } catch (ScannerException e)
            {
                HandleScannerException(e.Line, e.Message);
            }
        }

        internal static void HandleScannerException(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Report(int line, string where, string message)
        {
            Error?.Invoke(typeof(Interpreter), new InterpreterEventArgs($"[line {line}] Error{where}: {message}"));
        }

        public static event EventHandler<InterpreterEventArgs> Error;

        public static event EventHandler<InterpreterEventArgs> Status;
    }
}
