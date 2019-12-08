using System;

namespace LoxVM.Debugging
{
    class Disassembler
    {
        public static void Disassemble(Chunk chunk, string name)
        {
            Console.WriteLine($"== {name} ==");

            for (var offset = 0; offset < chunk.Size;)
            {
                offset = Disassemble(chunk, offset);
            }
        }

        public static int Disassemble(Chunk chunk, int offset)
        {
            Console.Write($"{offset.Format()} ");

            if (offset > 0 && chunk.Lines[offset] == chunk.Lines[offset - 1])
            {
                Console.Write("   | ");
            }
            else
            {
                Console.Write($"{chunk.Lines[offset].Format()} ");
            }

            var instruction = chunk[offset];

            switch (instruction)
            {
                case (byte)OpCode.CONSTANT:
                    return ConstantInstruction("OP_CONSTANT", chunk, offset);
                case (byte)OpCode.NIL:
                    return SimpleInstruction("OP_NIL", offset);
                case (byte)OpCode.TRUE:
                    return SimpleInstruction("OP_TRUE", offset);
                case (byte)OpCode.FALSE:
                    return SimpleInstruction("OP_FALSE", offset);
                case (byte)OpCode.EQUAL:
                    return SimpleInstruction("OP_EQUAL", offset);
                case (byte)OpCode.GREATER:
                    return SimpleInstruction("OP_GREATER", offset);
                case (byte)OpCode.LESS:
                    return SimpleInstruction("OP_LESS", offset);
                case (byte)OpCode.ADD:
                    return SimpleInstruction("OP_ADD", offset);
                case (byte)OpCode.SUBTRACT:
                    return SimpleInstruction("OP_SUBTRACT", offset);
                case (byte)OpCode.MULTIPLY:
                    return SimpleInstruction("OP_MULTIPLY", offset);
                case (byte)OpCode.DIVIDE:
                    return SimpleInstruction("OP_DIVIDE", offset);
                case (byte)OpCode.NOT:
                    return SimpleInstruction("OP_NOT", offset);
                case (byte)OpCode.NEGATE:
                    return SimpleInstruction("OP_NEGATE", offset);
                case (byte)OpCode.RETURN:
                    return SimpleInstruction("OP_RETURN", offset);
                default:
                    Console.WriteLine($"Unknown opcode: {instruction.Format()}");
                    return offset + 1;
            }
        }

        private static int ConstantInstruction(string name, Chunk chunk, int offset)
        {
            var constant = chunk[offset + 1];
            Console.WriteLine($"{name,-16} {((int)constant).Format()} '{chunk.Constants[constant].PrintValue()}'");
            return offset + 2;
        }

        private static int SimpleInstruction(string name, int offset)
        {
            Console.WriteLine(name);
            return offset + 1;
        }
    }
}
