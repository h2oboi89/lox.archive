using System;
using System.Collections.Generic;

namespace LoxVM
{
    static class Extensions
    {
        public static bool IsNumber(this object value)
        {
            return value.GetType() == typeof(double);
        }

        public static bool IsBoolean(this object value)
        {
            return value.GetType() == typeof(bool);
        }

        public static bool IsString(this object value)
        {
            return value.GetType() == typeof(string);
        }

        public static string PrintValue(this object value)
        {
            if (value == null)
            {
                return "nil";
            }
            else if (value.IsNumber())
            {
                return ((double)value).ToString("G");
            }
            else if (value.IsBoolean())
            {
                return (bool)value ? "true" : "false";
            }
            else
            {
                return value.ToString();
            }
        }

        public static object Peek(this Stack<object> stack, int depth = 0)
        {
            if (depth < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(depth));
            }

            var temp = new Stack<object>();

            while (depth-- > 0)
            {
                temp.Push(stack.Pop());
            }

            var t = stack.Peek();

            while (temp.Count > 0)
            {
                stack.Push(temp.Pop());
            }

            return t;
        }

        public static string Format(this byte b)
        {
            return "0x" + b.ToString("X2");
        }

        public static string Format(this int i)
        {
            return i.ToString("D04");
        }
    }
}
