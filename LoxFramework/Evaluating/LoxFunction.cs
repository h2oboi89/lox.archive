using LoxFramework.AST;
using LoxFramework.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace LoxFramework.Evaluating
{
    class LoxFunction : LoxCallable
    {
        private readonly Token thisToken = new Token(TokenType.THIS, "this");
        private readonly FunctionStatement declaration;
        private readonly Environment closure;

        public LoxFunction(FunctionStatement declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }

        public LoxFunction Bind(LoxInstance loxInstance)
        {
            var environment = new Environment(closure);
            environment.Define(thisToken, loxInstance);
            return new LoxFunction(declaration, environment);
        }

        public override int Arity()
        {
            return declaration.Parameters.Count();
        }

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
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return $"<function {declaration.Name.Lexeme}>";
        }
    }
}
