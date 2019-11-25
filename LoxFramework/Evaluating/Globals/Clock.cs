using System;
using System.Collections.Generic;

namespace LoxFramework.Evaluating.Globals
{
    class Clock : BuiltInLoxFunction
    {
        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
