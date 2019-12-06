using System;

namespace LoxVM
{
    static class Functor
    {
        public static Func<double, double, double> Add()
        {
            return (left, right) => left + right;
        }

        public static Func<double, double, double> Subtract()
        {
            return (left, right) => left - right;
        }

        public static Func<double, double, double> Multiply()
        {
            return (left, right) => left * right;
        }

        public static Func<double, double, double> Divide()
        {
            return (left, right) => left / right;
        }
    }
}
