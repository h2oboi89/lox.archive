﻿using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class Environment
    {
        private readonly Environment enclosing;
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private const int IGNORE = -1;

        public static bool PromptMode = false;

        public Environment(Environment enclosingEnvironment = null)
        {
            enclosing = enclosingEnvironment;
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
                if (values.ContainsKey(name.Lexeme))
                {
                    values[name.Lexeme] = value;
                    return;
                }

                if (enclosing != null)
                {
                    enclosing.Assign(name, value);
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
                environment = environment.enclosing;
            }

            return environment;
        }

        public object Get(Token name, int distance = IGNORE)
        {
            if (distance == IGNORE)
            {
                if (values.ContainsKey(name.Lexeme))
                {
                    return values[name.Lexeme];
                }

                if (enclosing != null) return enclosing.Get(name);

                throw new LoxRunTimeException(name, $"Undefined variable '{name.Lexeme}'.");
            }
            else
            {
                return Ancestor(distance).values[name.Lexeme];
            }
        }
    }
}
