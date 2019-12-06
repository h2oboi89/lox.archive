using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LoxVM
{
    class Program
    {
        static void Main(string[] args)
        {
            var vm = new VirtualMachine();

            var chunk = new Chunk();

            chunk.AddConstant(1.2, 1);
            chunk.AddConstant(3.4, 1);

            chunk.AddInstruction(OpCode.ADD, 1);

            chunk.AddConstant(5.6, 1);

            chunk.AddInstruction(OpCode.DIVIDE, 1);

            chunk.AddInstruction(OpCode.NEGATE, 1);

            chunk.AddInstruction(OpCode.RETURN, 1);

#if DEBUG
            Disassembler.Disassemble(chunk, "test chunk");
#endif

            vm.Interpret(chunk);

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
