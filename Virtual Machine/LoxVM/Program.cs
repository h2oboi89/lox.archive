using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoxVM
{
    class Program
    {
        static void Main(string[] args)
        {
            var chunk = new Chunk();

            chunk.AddConstant(1.2, 1);
            chunk.AddInstruction(OpCode.RETURN, 1);

            Disassembler.Disassemble(chunk, "test chunk");

            if (Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }

            Environment.Exit(0);
        }

        private static void AddInstruction(List<byte> chunk, OpCode opcode)
        {
            chunk.Add((byte)opcode);
        }
    }
}
