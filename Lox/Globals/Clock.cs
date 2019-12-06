using System;
using System.Collections.Generic;

namespace Lox.Globals
{
    class Clock : LoxBuiltInFunction
    {
        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
