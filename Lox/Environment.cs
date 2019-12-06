using LoxFramework.Scanning;
using System.Collections.Generic;

namespace Lox
{
    class Environment
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private const int IGNORE = int.MinValue;
        public static bool PromptMode = false;

        public Environment Enclosing { get; private set; }

        public Environment(Environment enclosing = null)
        {
            Enclosing = enclosing;
        }

        public void Define(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                if (PromptMode)
                {
                    values[name.Lexeme] = value;
                }
                else
                {
                    throw new LoxRunTimeException(name, $"Variable '{name.Lexeme}' already declared in this scope.");
                }
            }
            else
            {
                values.Add(name.Lexeme, value);
            }
        }

        public void Assign(Token name, object value, int distance = IGNORE)
        {
            if (distance == IGNORE)
            {
                // working in global scope
                if (values.ContainsKey(name.Lexeme))
                {
                    values[name.Lexeme] = value;
                    return;
                }

                throw new LoxRunTimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }
            else
            {
                Ancestor(distance).values[name.Lexeme] = value;
            }
        }

        private Environment Ancestor(int distance)
        {
            var environment = this;

            for (var i = 0; i < distance; i++)
            {
                environment = environment.Enclosing;
            }

            return environment;
        }

        public object Get(Token name, int distance = IGNORE)
        {
            if (distance == IGNORE)
            {
                // working in global scope
                if (values.ContainsKey(name.Lexeme))
                {
                    return values[name.Lexeme];
                }

                throw new LoxRunTimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }
            else
            {
                return Ancestor(distance).values[name.Lexeme];
            }
        }
    }
}
