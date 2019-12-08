using System.Collections.Generic;

namespace LoxVM
{
    class Chunk
    {
        private readonly List<byte> code = new List<byte>();
        private readonly List<object> constants = new List<object>();
        private readonly List<int> lines = new List<int>();

        public IReadOnlyList<byte> Code { get { return code.AsReadOnly(); } }

        public IReadOnlyList<object> Constants { get { return constants.AsReadOnly(); } }

        public IReadOnlyList<int> Lines { get { return lines.AsReadOnly(); } }

        public int Size { get { return code.Count; } }

        public byte this[int i]
        {
            get { return code[i]; }
        }

        private void AddByte(byte b, int line)
        {
            code.Add(b);
            lines.Add(line);
        }

        public void AddOpCode(int line, params OpCode[] opcodes)
        {
            foreach (var opcode in opcodes)
            {
                AddByte((byte)opcode, line);
            }
        }

        public void AddConstant(object value, int line)
        {
            // TODO: throw exception if greater than 256 constants
            constants.Add(value);

            var index = constants.Count - 1;

            AddByte((byte)OpCode.CONSTANT, line);
            AddByte((byte)index, line);
        }
    }
}
