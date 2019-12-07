using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System;

namespace LoxVM
{
    static class Compiler
    {
        public static Chunk Compile(string source)
        {
            var tokens = Scanner.Scan(source);

            // TODO: check for scan errors

            var statements = Parser.Parse(tokens);

            // TODO: check for parser errors

            // TODO: iterate through statements and generate chunk(s)

            throw new NotImplementedException();
        }
    }
}
