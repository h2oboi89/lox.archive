using LoxFramework.Scanning;
using System;

namespace LoxVM
{
    static class Compiler
    {
        public static Chunk Compile(string source)
        {
            var tokens = Scanner.Scan(source);

            throw new NotImplementedException();
        }
    }
}
