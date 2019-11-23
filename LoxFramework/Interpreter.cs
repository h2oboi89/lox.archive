using LoxFramework.Evaluating;
using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System;
using System.Linq;

namespace LoxFramework
{
    /// <summary>
    /// Lox interpreter.
    /// </summary>
    public static class Interpreter
    {
        private static readonly AstInterpreter astInterpreter = new AstInterpreter();
        private static bool HadError = false;
        private static bool Initialized = false;

        /// <summary>
        /// R
        /// </summary>
        public static void Reset()
        {
            astInterpreter.Reset();
        }

        private static void Initialize()
        {
            astInterpreter.Out += (o, e) => Out?.Invoke(typeof(Interpreter), e);

            Initialized = true;
        }

        /// <summary>
        /// Executes the specified source code.
        /// </summary>
        /// <param name="source">Source code to execute.</param>
        public static void Run(string source)
        {
            if (!Initialized) Initialize();

            HadError = false;

            var tokens = Scanner.Scan(source);

            // check for empty input (EOF token)
            if (!HadError && tokens.Count() == 1) return;

            var statements = Parser.Parse(tokens);

            // check for parse error
            if (HadError) return;

            astInterpreter.Interpret(statements);
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

        internal static void InterpretError(LoxRunTimeException e)
        {
            Report(e.Token.Line, $" at {e.Token.Lexeme}", e.Message);
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
