using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class Environment
    {
        private readonly Environment _enclosing;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public Environment(Environment enclosing = null)
        {
            _enclosing = enclosing;
        }

        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (_enclosing != null)
            {
                _enclosing.Assign(name, value);
                return;
            }

            throw new LoxRunTimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            if (_enclosing != null) return _enclosing.Get(name);

            throw new LoxRunTimeException(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
