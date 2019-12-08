﻿using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System;
using System.Linq;

namespace Lox
{
    /// <summary>
    /// Lox interpreter.
    /// </summary>
    public static class Interpreter
    {
        private static readonly AstInterpreter astInterpreter = new AstInterpreter();
        private static bool hadError = false;
        private static bool initialized = false;

        /// <summary>
        /// Reset interpreter environment (mainly used for testing) 
        /// </summary>
        /// <param name="promptMode">If true then global variables can be redeclared; otherwise that throws a run time exception</param>
        public static void Reset(bool promptMode = false)
        {
            Environment.PromptMode = promptMode;

            astInterpreter.Reset();
        }

        private static void Initialize(bool promptMode)
        {
            Environment.PromptMode = promptMode;

            astInterpreter.Out += (o, e) => Out?.Invoke(typeof(Interpreter), e);

            initialized = true;
        }

        /// <summary>
        /// Executes the specified source code.
        /// </summary>
        /// <param name="source">Source code to execute.</param>
        /// <param name="promptMode">True if running from prompt; otherwise false.</param>
        public static void Run(string source, bool promptMode = false)
        {
            if (!initialized) Initialize(promptMode);

            hadError = false;

            var tokens = Scanner.Scan(source).ToList();

            foreach (var error in Scanner.Errors)
            {
                Report(error.Line, "", error.Message);
            }

            // check for empty input (EOF token)
            if (!hadError && tokens.Count() == 1) return;

            var statements = Parser.Parse(tokens).ToList();

            foreach (var error in Parser.Errors)
            {
                AstError(error.Token, error.Message);
            }

            // check for parse error
            if (hadError) return;

            Resolver.Resolve(astInterpreter, statements);

            // check for resolve error
            if (hadError) return;

            astInterpreter.Interpret(statements);
        }

        private static void Report(int line, string where, string message)
        {
            Error?.Invoke(typeof(Interpreter), new InterpreterEventArgs($"[line {line}] Error{where}: {message}"));
            hadError = true;
        }

        internal static void AstError(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, $" at '{token.Lexeme}'", message);
            }
        }

        internal static void RunTimeError(LoxRunTimeException e)
        {
            Report(e.Token.Line, $" at '{e.Token.Lexeme}'", e.Message);
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
