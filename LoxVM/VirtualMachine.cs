using LoxVM.Compiling;
using LoxVM.Debug;
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
                        case (byte)OpCode.EQUAL:
                            var b = stack.Pop();
                            var a = stack.Pop();
                            stack.Push(AreEqual(a, b));
                            break;
                        case (byte)OpCode.GREATER: BinaryOperation(Functor.Greater()); break;
                        case (byte)OpCode.LESS: BinaryOperation(Functor.Less()); break;
                        case (byte)OpCode.ADD:
                            if (stack.Peek().IsString() && stack.Peek(1).IsString())
                            {
                                Concatenate();
                            }
                            else if (stack.Peek().IsNumber() && stack.Peek(1).IsNumber())
                            {
                                BinaryOperation(Functor.Add());
                            }
                            else
                            {
                                RuntimeError("Operands must be two numbers or two strings.");
                            }
                            break;
                        case (byte)OpCode.SUBTRACT: BinaryOperation(Functor.Subtract()); break;
                        case (byte)OpCode.MULTIPLY: BinaryOperation(Functor.Multiply()); break;
                        case (byte)OpCode.DIVIDE: BinaryOperation(Functor.Divide()); break;
                        case (byte)OpCode.NOT: stack.Push(IsFalsey(stack.Pop())); break;
                        case (byte)OpCode.NEGATE:
                            if (!stack.Peek().IsNumber())
                            {
                                RuntimeError("Operand must be a number.");
                            }
                            stack.Push(-(double)stack.Pop());
                            break;
                        case (byte)OpCode.RETURN:
                            Console.WriteLine($"{stack.Pop().PrintValue()}");
                            return Result.OK;
                    }
                }
                catch (Exception)
                {
                    return Result.RUNTIME_ERROR;
                }
            }
        }

        #region Utility Methods
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

        private object ReadConstant()
        {
            return chunk.Constants[Fetch()];
        }

        private bool IsFalsey(object value)
        {
            if (value == null) return true;
            if (value.IsBoolean()) return !(bool)value;
            return false;
        }

        private bool AreEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void Concatenate()
        {
            var b = (string)stack.Pop();
            var a = (string)stack.Pop();

            stack.Push(a + b);
        }

        private void CheckOperands(object left, object right)
        {
            if (!right.IsNumber() || !left.IsNumber())
            {
                RuntimeError("Operands must be numbers.");
            }
        }

        private void BinaryOperation(Func<double, double, bool> op)
        {
            var right = stack.Pop();
            var left = stack.Pop();

            CheckOperands(left, right);

            stack.Push(op((double)left, (double)right));
        }

        private void BinaryOperation(Func<double, double, double> op)
        {
            var right = stack.Pop();
            var left = stack.Pop();

            CheckOperands(left, right);

            stack.Push(op((double)left, (double)right));
        }

        private void PrintStack()
        {
            Console.Write("          ");
            foreach (var value in stack)
            {
                Console.Write($"[ {value.PrintValue()} ]");
            }
            Console.WriteLine();
        }
        #endregion
    }
}
