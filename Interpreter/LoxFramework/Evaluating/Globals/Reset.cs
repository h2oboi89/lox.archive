using System.Collections.Generic;

namespace LoxFramework.Evaluating.Globals
{
    class Reset : BuiltInLoxFunction
    {
        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            interpreter.Reset();

            return null;
        }
    }
}
