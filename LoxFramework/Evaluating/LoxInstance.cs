using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class LoxInstance
    {
        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();
        private readonly LoxClass loxClass;


        public LoxInstance(LoxClass loxClass)
        {
            this.loxClass = loxClass;
        }

        public object this[Token name]
        {
            get
            {
                if (fields.ContainsKey(name.Lexeme))
                {
                    return fields[name.Lexeme];
                }

                var method = loxClass[name.Lexeme];
                if (method != null) return method.Bind(this);

                throw new LoxRunTimeException(name, $"Undefined property '{name.Lexeme}'.");
            }
            set
            {
                if (!fields.ContainsKey(name.Lexeme))
                {
                    fields.Add(name.Lexeme, value);
                }
                else
                {
                    fields[name.Lexeme] = value;
                }
            }
        }

        public override string ToString()
        {
            return $"{loxClass.Name} instance";
        }
    }
}
