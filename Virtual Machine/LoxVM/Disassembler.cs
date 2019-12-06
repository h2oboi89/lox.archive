using System;

namespace LoxVM
{
    class Disassembler
    {
        public static void Disassemble(Chunk chunk, string name)
        {
            Console.WriteLine($"== {name} ==");

            for (var offset = 0; offset < chunk.Count;)
            {
                offset = Disassemble(chunk, offset);
            }
        }

        private static int Disassemble(Chunk chunk, int offset)
        {
            Console.Write($"{FormatInt(offset)} ");

            if (offset > 0 && chunk.Lines[offset] == chunk.Lines[offset - 1])
            {
                Console.Write("   | ");
            }
            else
            {
                Console.Write($"{FormatInt(chunk.Lines[offset])} ");
            }

            var instruction = chunk[offset];

            switch (instruction)
            {
                case (byte)OpCode.CONSTANT:
                    return ConstantInstruction("OP_CONSTANT", chunk, offset);
                case (byte)OpCode.RETURN:
                    return SimpleInstruction("OP_RETURN", offset);
                default:
                    Console.WriteLine($"Unknown opcode: {FormatByte(instruction)}");
                    return offset + 1;
            }
        }

        private static int ConstantInstruction(string name, Chunk chunk, int offset)
        {
            var constant = chunk[offset + 1];
            Console.WriteLine($"{name,-16} {FormatInt(constant)} '{PrintValue(chunk.Constants[constant])}'");
            return offset + 2;
        }

        private static int SimpleInstruction(string name, int offset)
        {
            Console.WriteLine(name);
            return offset + 1;
        }

        private static string FormatByte(byte b)
        {
            return "0x" + b.ToString("X2");
        }

        private static string FormatInt(int i)
        {
            return i.ToString("D04");
        }

        private static string PrintValue(double value)
        {
            return value.ToString("G");
        }
    }
}
