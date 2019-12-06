using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace Lox
{
    class LoxFunction : LoxCallable
    {
        private readonly Token thisToken = new Token(TokenType.THIS, "this");
        private readonly FunctionStatement declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;

        public LoxFunction(FunctionStatement declaration, Environment closure, bool isInitializer = false)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitializer;
        }

        public LoxFunction Bind(LoxInstance loxInstance)
        {
            var environment = new Environment(closure);
            environment.Define(thisToken, loxInstance);
            return new LoxFunction(declaration, environment, isInitializer);
        }

        public override int Arity { get { return declaration.Parameters.Count(); } }

        private object InstanceReference { get { return closure.Get(thisToken, 0); } }

        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            var environment = new Environment(closure);

            foreach (var parameter in declaration.Parameters.Enumerate())
            {
                environment.Define(parameter.Value, arguments.ElementAt(parameter.Index));
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (LoxReturn returnValue)
            {
                return isInitializer ? InstanceReference : returnValue.Value;
            }

            if (isInitializer) return InstanceReference;

            return null;
        }

        public override string ToString()
        {
            return $"<function {declaration.Name.Lexeme}>";
        }
    }
}
