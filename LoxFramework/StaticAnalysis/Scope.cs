using LoxFramework.AST;
using LoxFramework.Evaluating;
using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.StaticAnalysis
{
    /// <summary>
    /// Utility class for <see cref="Resolver"/>.
    /// Helps keep track of current scopes.
    /// </summary>
    class Scope
    {
        private class ScopeLevel
        {
            private readonly Dictionary<string, bool> values = new Dictionary<string, bool>();

            public bool Declare(string name)
            {
                if (values.ContainsKey(name))
                {
                    return false;
                }

                values.Add(name, false);

                return true;
            }

            public void Define(string name)
            {
                values[name] = true;
            }

            public void Initialize(string name)
            {
                Declare(name);
                Define(name);
            }

            public bool IsDeclared(string name)
            {
                return values.ContainsKey(name);
            }

            public bool IsDefined(string name)
            {
                return IsDeclared(name) && values[name] == true;
            }
        }

        /// <summary>
        /// Function scope types.
        /// </summary>
        public enum FunctionType
        {
            /// <summary>
            /// Lox function
            /// </summary>
            Function,
            /// <summary>
            /// Lox class method
            /// </summary>
            Method
        }

        private readonly Stack<FunctionType> currentFunction = new Stack<FunctionType>();
        private readonly LinkedList<ScopeLevel> scopes = new LinkedList<ScopeLevel>();
        private readonly AstInterpreter interpreter;

        public Scope(AstInterpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        private int loopLevel = 0;

        /// <summary>
        /// Enters a new loop scope.
        /// </summary>
        public void EnterLoop() { loopLevel++; }

        /// <summary>
        /// Exits the current loop scope and returns to the previous loop scope.
        /// </summary>
        public void ExitLoop() { loopLevel--; }

        /// <summary>
        /// True if we are in a loop; otherwise false.
        /// </summary>
        public bool InLoop { get { return loopLevel > 0; } }

        private bool IsEmpty { get { return scopes.Count == 0; } }

        /// <summary>
        /// Enters a new scope level
        /// </summary>
        public void Enter()
        {
            scopes.AddLast(new ScopeLevel());
        }

        /// <summary>
        /// Exits the current scope level and returns to previous scope level
        /// </summary>
        public void Exit()
        {
            scopes.RemoveLast();
        }

        /// <summary>
        /// Enters a new function scope.
        /// </summary>
        /// <param name="type">Function type</param>
        public void EnterFunction(FunctionType type)
        {
            currentFunction.Push(type);
            Enter();
        }

        /// <summary>
        /// Exits the current function scope and returns to previous scope level
        /// </summary>
        public void ExitFunction()
        {
            Exit();
            currentFunction.Pop();
        }

        /// <summary>
        /// True if in function scope; otherwise false.
        /// </summary>
        public bool InFunction
        {
            get { return currentFunction.Count > 0; }
        }

        /// <summary>
        /// Enters a new class scope.
        /// </summary>
        public void EnterClass()
        {
            Enter();
            scopes.Last.Value.Initialize("this");
        }

        /// <summary>
        /// Exits the current class scope and returns to previous scope level
        /// </summary>
        public void ExitClass()
        {
            Exit();
        }

        /// <summary>
        /// Attempts to declare a value in the current scope
        /// </summary>
        /// <param name="name">Value to declare</param>
        public void Declare(Token name)
        {
            if (IsEmpty) return;

            if (!scopes.Last.Value.Declare(name.Lexeme))
            {
                Interpreter.ScopeError(name, "Variable with this name already declared in this scope.");
            }
        }

        /// <summary>
        /// Defines a declared value in the current scope
        /// </summary>
        /// <param name="name">Value to define</param>
        public void Define(Token name)
        {
            if (IsEmpty) return;

            scopes.Last.Value.Define(name.Lexeme);
        }

        /// <summary>
        /// Combines <see cref="Declare(Token)"/> and <see cref="Define(Token)"/> into a single operation.
        /// </summary>
        /// <param name="name">Value to declare and define</param>
        public void Initialize(Token name)
        {
            Declare(name);
            Define(name);
        }

        /// <summary>
        /// Checks if value is declared in the current scope.
        /// </summary>
        /// <param name="name">Value to check.</param>
        /// <returns>True if declared in current scope; otherwise false.</returns>
        public bool IsDeclared(Token name)
        {
            return IsEmpty || scopes.Last.Value.IsDeclared(name.Lexeme);
        }

        /// <summary>
        /// Checks if a value is declared and defined in the current scope.
        /// </summary>
        /// <param name="name">Value to check.</param>
        /// <returns>True if declared and defined; otherwise false.</returns>
        public bool IsDefined(Token name)
        {
            return IsEmpty || scopes.Last.Value.IsDefined(name.Lexeme);
        }

        /// <summary>
        /// Resolves a value
        /// </summary>
        /// <param name="expression">Expression with value to resolve</param>
        /// <param name="name">Value to resolve</param>
        public void ResolveValue(Expression expression, Token name)
        {
            var distance = 0;
            var scope = scopes.Last;

            while (scope != null)
            {
                if (scope.Value.IsDeclared(name.Lexeme))
                {
                    interpreter.Resolve(expression, distance);
                    break;
                }

                scope = scope.Previous;
                distance++;
            }

            // not found. Assume it is global
        }
    }
}
