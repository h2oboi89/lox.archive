using LoxFramework.AST;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class LoxClass : LoxCallable
    {
        private const string INIT = "init";

        public static bool IsInitializer(FunctionStatement method)
        {
            return method.Name.Lexeme == INIT;
        }

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

        private LoxFunction Constructor
        {
            get { return this[INIT]; }
        }

        public override int Arity { get { return Constructor?.Arity ?? 0; } }

        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            var instance = new LoxInstance(this);

            Constructor?.Bind(instance).Call(interpreter, arguments);

            return instance;
        }
    }
}
