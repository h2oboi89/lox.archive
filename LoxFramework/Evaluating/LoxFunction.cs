using LoxFramework.AST;
using System.Collections.Generic;
using System.Linq;

namespace LoxFramework.Evaluating
{
    class LoxFunction : LoxCallable
    {
        private readonly FunctionStatement declaration;

        public LoxFunction(FunctionStatement declaration)
        {
            this.declaration = declaration;
        }

        public override int Arity()
        {
            return declaration.Parameters.Count();
        }

        public override object Call(AstInterpreter interpreter, IEnumerable<object> arguments)
        {
            var environment = new Environment(interpreter.Globals);

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
