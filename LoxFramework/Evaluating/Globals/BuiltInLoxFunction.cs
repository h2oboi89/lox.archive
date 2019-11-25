using System;
using System.Collections.Generic;

namespace LoxFramework.Evaluating.Globals
{
    class BuiltInLoxFunction : LoxCallable
    {
        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "<function native>";
        }
    }
}
