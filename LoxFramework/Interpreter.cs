using System;

namespace LoxFramework
{
    public static class Interpreter
    {
        public static void Run(string source)
        {
            var scanner = new Scanner(source);

            try
            {
                var tokens = scanner.ScanTokens();

                foreach (var token in tokens)
                {
                    OnStatus?.Invoke(null, new InterpreterEventArgs(token.ToString()));
                }
            } catch (ScannerException e)
            {
                Error(e.Line, e.Message);
            }
        }

        internal static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void Report(int line, string where, string message)
        {
            OnError?.Invoke(null, new InterpreterEventArgs($"[line {line}] Error{where}: {message}"));
        }

        public static event EventHandler<InterpreterEventArgs> OnError;

        public static event EventHandler<InterpreterEventArgs> OnStatus;
    }
}
