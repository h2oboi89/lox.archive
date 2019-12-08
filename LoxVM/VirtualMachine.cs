using System;
using System.Collections.Generic;

namespace LoxVM
{
    class VirtualMachine
    {
        public enum Result
        {
            OK,
            NOOP,
            COMPILE_ERROR,
            RUNTIME_ERROR
        }

        private Chunk chunk;
        private int InstructionPointer;
        private readonly Stack<double> stack = new Stack<double>();

        public Result Interpret(string source)
        {
            try
            {
                if (string.IsNullOrEmpty(source))
                {
                    return Result.NOOP;
                }

                chunk = Compiler.Compile(source);
                InstructionPointer = 0;

                return Execute();
            }
            catch (CompileException)
            {
                return Result.COMPILE_ERROR;
            }
        }

        private byte Fetch()
        {
            return chunk.Code[InstructionPointer++];
        }

        private double ReadConstant()
        {
            return chunk.Constants[Fetch()];
        }

        private Result Execute()
        {
            for (; ; )
            {
#if DEBUG
                PrintStack();

                Disassembler.Disassemble(chunk, InstructionPointer);
#endif

                var instruction = Fetch();

                switch (instruction)
                {
                    case (byte)OpCode.CONSTANT:
                        stack.Push(ReadConstant());
                        break;
                    case (byte)OpCode.ADD: BinaryOperation(Functor.Add()); break;
                    case (byte)OpCode.SUBTRACT: BinaryOperation(Functor.Subtract()); break;
                    case (byte)OpCode.MULTIPLY: BinaryOperation(Functor.Multiply()); break;
                    case (byte)OpCode.DIVIDE: BinaryOperation(Functor.Divide()); break;
                    case (byte)OpCode.NEGATE:
                        stack.Push(-stack.Pop());
                        break;
                    case (byte)OpCode.RETURN:
                        Console.WriteLine($"{PrintValue(stack.Pop())}");
                        return Result.OK;
                }
            }
        }

        private void BinaryOperation(Func<double, double, double> op)
        {
            var right = stack.Pop();
            var left = stack.Pop();

            stack.Push(op(left, right));
        }

        private void PrintStack()
        {
            Console.Write("          ");
            foreach (var v in stack)
            {
                Console.Write($"[ {PrintValue(v)} ]");
            }
            Console.WriteLine();
        }

        private static string PrintValue(double value)
        {
            return value.ToString("G");
        }
    }
}
