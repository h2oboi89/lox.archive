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
        private readonly Stack<object> stack = new Stack<object>();

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

        private void RuntimeError(string message)
        {
            var line = chunk.Lines[InstructionPointer];

            Console.Error.WriteLine(message);
            Console.Error.WriteLine($"[line {line}] in script");

            throw new Exception();
        }

        private byte Fetch()
        {
            return chunk.Code[InstructionPointer++];
        }

        private double ReadConstant()
        {
            return chunk.Constants[Fetch()];
        }

        private bool IsNumber(object value)
        {
            return value.GetType() == typeof(double);
        }

        private bool IsBoolean(object value)
        {
            return value.GetType() == typeof(bool);
        }

        private bool IsFalsey(object value)
        {
            if (value == null) return true;
            if (IsBoolean(value)) return !(bool)value;
            return false;
        }

        private Result Execute()
        {
            while (true)
            {
#if DEBUG
                PrintStack();

                Disassembler.Disassemble(chunk, InstructionPointer);
#endif

                var instruction = Fetch();
                try
                {
                    switch (instruction)
                    {
                        case (byte)OpCode.CONSTANT: stack.Push(ReadConstant()); break;
                        case (byte)OpCode.NIL: stack.Push(null); break;
                        case (byte)OpCode.TRUE: stack.Push(true); break;
                        case (byte)OpCode.FALSE: stack.Push(false); break;
                        case (byte)OpCode.ADD: BinaryOperation(Functor.Add()); break;
                        case (byte)OpCode.SUBTRACT: BinaryOperation(Functor.Subtract()); break;
                        case (byte)OpCode.MULTIPLY: BinaryOperation(Functor.Multiply()); break;
                        case (byte)OpCode.DIVIDE: BinaryOperation(Functor.Divide()); break;
                        case (byte)OpCode.NOT: stack.Push(IsFalsey(stack.Pop())); break;
                        case (byte)OpCode.NEGATE:
                            if (!IsNumber(stack.Peek()))
                            {
                                RuntimeError("Operand must be a number.");
                            }
                            stack.Push(-(double)stack.Pop());
                            break;
                        case (byte)OpCode.RETURN:
                            Console.WriteLine($"{PrintValue(stack.Pop())}");
                            return Result.OK;
                    }
                }
                catch (Exception)
                {
                    return Result.RUNTIME_ERROR;
                }
            }
        }

        private void BinaryOperation(Func<double, double, double> op)
        {
            var right = stack.Pop();
            var left = stack.Pop();

            if (!IsNumber(right) || !IsNumber(left))
            {
                RuntimeError("Operands must be numbers.");
            }

            stack.Push(op((double)left, (double)right));
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

        private string PrintValue(object value)
        {
            if (value == null)
            {
                return "nil";
            }
            else if (IsNumber(value))
            {
                return ((double)value).ToString("G");
            }
            else if (IsBoolean(value))
            {
                return (bool)value ? "true" : "false";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
