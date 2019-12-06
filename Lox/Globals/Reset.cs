using System.Collections.Generic;

namespace Lox.Globals
{
    class Reset : LoxBuiltInFunction
    {
        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            interpreter.Reset();

            return null;
        }
    }
}
