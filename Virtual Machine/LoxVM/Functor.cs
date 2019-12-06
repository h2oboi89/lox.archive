using System;

namespace LoxVM
{
    static class Functor
    {
        public static Func<double, double, double> Add()
        {
            return delegate (double left, double right) { return left + right; };
        }

        public static Func<double, double, double> Subtract()
        {
            return delegate (double left, double right) { return left - right; };
        }

        public static Func<double, double, double> Multiply()
        {
            return delegate (double left, double right) { return left * right; };
        }

        public static Func<double, double, double> Divide()
        {
            return delegate (double left, double right) { return left / right; };
        }
    }
}
