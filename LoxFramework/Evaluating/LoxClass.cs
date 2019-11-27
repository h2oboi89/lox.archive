using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class LoxClass : LoxCallable
    {
        public string Name { get; private set; }

        public LoxClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            var instance = new LoxInstance(this);

            return instance;
        }
    }
}
