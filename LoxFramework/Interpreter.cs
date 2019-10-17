using System;

namespace LoxFramework
{
    /// <summary>
    /// Lox interpreter.
    /// </summary>
    public static class Interpreter
    {
        /// <summary>
        /// Executes the specified source code.
        /// </summary>
        /// <param name="source">Source code to execute.</param>
        public static void Run(string source)
        {
            try
            {
                var tokens = Scanner.Scan(source);

                foreach (var token in tokens)
                {
                    Out?.Invoke(null, new InterpreterEventArgs(token.ToString()));
                }
            } catch (ScannerException e)
            {
                HandleScannerException(e.Line, e.Message);
            }
        }

        private static void HandleScannerException(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Error?.Invoke(typeof(Interpreter), new InterpreterEventArgs($"[line {line}] Error{where}: {message}"));
        }

        /// <summary>
        /// Event fired when interpreter encounters an error.
        /// </summary>
        public static event EventHandler<InterpreterEventArgs> Error;

        /// <summary>
        /// Event fired when interpreter has output.
        /// </summary>
        public static event EventHandler<InterpreterEventArgs> Out;
    }
}
