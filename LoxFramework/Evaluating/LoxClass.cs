using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class LoxClass : LoxCallable
    {
        public string Name { get; private set; }
        private readonly Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            this.methods = methods;
        }

        public LoxFunction this[string name]
        {
            get
            {
                if (methods.ContainsKey(name))
                {
                    return methods[name];
                }

                return null;
            }
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
