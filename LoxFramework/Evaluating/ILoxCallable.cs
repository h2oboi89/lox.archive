using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    interface ILoxCallable
    {
        object Call(AstInterpreter interpreter, IEnumerable<object> arguments);

        int Arity();
    }
}
