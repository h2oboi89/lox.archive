using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    interface ILoxCallable
    {
        int Arity();

        object Call(AstInterpreter interpreter, IEnumerable<object> arguments);
    }

    abstract class LoxCallable : ILoxCallable
    {
        public virtual int Arity() { return 0; }

        public abstract object Call(AstInterpreter interpreter, IEnumerable<object> arguments);
    }
}
