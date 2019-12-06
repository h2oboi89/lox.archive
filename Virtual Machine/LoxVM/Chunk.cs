using System.Collections.Generic;

namespace LoxVM
{
    class Chunk
    {
        private readonly List<byte> code = new List<byte>();
        private readonly List<double> constants = new List<double>();
        private readonly List<int> lines = new List<int>();

        public int Count { get { return code.Count; } }

        public byte this[int i]
        {
            get { return code[i]; }
        }

        private void AddByte(byte b, int line)
        {
            code.Add(b);
            lines.Add(line);
        }

        public void AddInstruction(OpCode opcode, int line)
        {
            AddByte((byte)opcode, line);
        }

        public void AddConstant(double value, int line)
        {
            constants.Add(value);

            var index = constants.Count - 1;

            AddByte((byte)OpCode.CONSTANT, line);
            AddByte((byte)index, line);
        }

        public IReadOnlyList<double> Constants { get { return constants.AsReadOnly(); } }

        public IReadOnlyList<int> Lines { get { return lines.AsReadOnly(); } }
    }
}
