using LoxFramework.AST;
using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System;

namespace LoxFramework
{
    /// <summary>
    /// Lox interpreter.
    /// </summary>
    public static class Interpreter
    {
        private static bool HadError = false;

        /// <summary>
        /// Executes the specified source code.
        /// </summary>
        /// <param name="source">Source code to execute.</param>
        public static void Run(string source)
        {
            HadError = false;

            var tokens = Scanner.Scan(source);
            var parser = new Parser(tokens);
            var expression = parser.Parse();

            if (HadError) return;

            Out?.Invoke(typeof(Interpreter), new InterpreterEventArgs(new AstPrinter().Print(expression)));
        }

        private static void Report(int line, string where, string message)
        {
            Error?.Invoke(typeof(Interpreter), new InterpreterEventArgs($"[line {line}] Error{where}: {message}"));
            HadError = true;
        }

        internal static void ScanError(int line, string message)
        {
            Report(line, "", message);
        }

        internal static void ParseError(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at {token.Lexeme}'", message);
            }
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
