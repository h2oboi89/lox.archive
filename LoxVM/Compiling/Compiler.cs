using Scanning;
using System;
using System.Linq;

namespace LoxVM.Compiling
{
    static class Compiler
    {
        private static bool hadError;

        public static Chunk Compile(string source)
        {
            hadError = false;

            var tokens = Scanner.Scan(source).ToList();

            foreach (var error in Scanner.Errors)
            {
                Report(error.Line, "", error.Message);
            }

            if (hadError) throw new CompileException();

            var chunk = Parser.Parse(tokens);

            foreach (var error in Parser.Errors)
            {
                ParseError(error.Token, error.Message);
            }

            if (hadError) throw new CompileException();

            return chunk;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }

        internal static void ParseError(Token token, string message)
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
    }
}
