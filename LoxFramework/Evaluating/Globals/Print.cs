using System.Collections.Generic;
using System.Linq;

namespace LoxFramework.Evaluating.Globals
{
    class Print : BuiltInLoxFunction
    {
        public override int Arity { get { return 1; } }

        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            interpreter.RaiseOut(arguments.First());

            return null;
        }
    }
}
