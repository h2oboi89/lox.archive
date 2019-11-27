using LoxFramework.AST;
using LoxFramework.Evaluating;
using LoxFramework.Scanning;
using System.Collections.Generic;
using System.Linq;

namespace LoxFramework.StaticAnalysis
{
    class Resolver : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly AstInterpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.None;
        private int inLoop = 0;

        private enum FunctionType
        {
            None,
            Function
        }

        private Resolver(AstInterpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public static void Resolve(AstInterpreter interpreter, IEnumerable<Statement> statements)
        {
            var resolver = new Resolver(interpreter);

            resolver.Resolve(statements);
        }

        #region Utility Methods
        private void Resolve(IEnumerable<Statement> statements)
        {
            foreach (var statement in statements)
            {
                Resolve(statement);
            }
        }

        private void Resolve(Statement statement)
        {
            statement?.Accept(this);
        }

        private void Resolve(Expression expression)
        {
            expression?.Accept(this);
        }

        private void ResolveLocal(Expression expression, Token name)
        {
            for (var i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expression, scopes.Count() - 1 - i);
                    return;
                }
            }

            // Not found. Assume it is global.
        }

        private void ResolveFunction(FunctionStatement function, FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;

            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();

            currentFunction = enclosingFunction;
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (scopes.IsEmpty()) return;

            var scope = scopes.Peek();

            if (scope.ContainsKey(name.Lexeme))
            {
                Interpreter.ResolutionError(name, "Variable with this name already declared in this scope.");
            }

            scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if (scopes.IsEmpty()) return;

            // TODO: check if token exists?
            scopes.Peek()[name.Lexeme] = true;
        }
        #endregion

        #region Statements
        public object VisitBlockStatement(BlockStatement statement)
        {
            BeginScope();
            Resolve(statement.Statements);
            EndScope();

            return null;
        }

        public object VisitFunctionStatement(FunctionStatement statement)
        {
            Declare(statement.Name);
            Define(statement.Name);

            ResolveFunction(statement, FunctionType.Function);

            return null;
        }

        public object VisitVariableStatement(VariableStatement statement)
        {
            Declare(statement.Name);
            Resolve(statement.Initializer);
            Define(statement.Name);

            return null;
        }

        #endregion

        #region Expressions
        public object VisitAssignmentExpression(AssignmentExpression expression)
        {
            Resolve(expression.Value);
            ResolveLocal(expression, expression.Name);

            return null;
        }

        public object VisitVariableExpression(VariableExpression expression)
        {
            if (!scopes.IsEmpty() && scopes.Peek()[expression.Name.Lexeme] == false)
            {
                Interpreter.ResolutionError(expression.Name, "Cannot read local variable in its own initializer.");
            }

            ResolveLocal(expression, expression.Name);

            return null;
        }
        #endregion

        #region Simple Traversal
        public object VisitBinaryExpression(BinaryExpression expression)
        {
            Resolve(expression.Left);
            Resolve(expression.Right);

            return null;
        }


        public object VisitBreakStatement(BreakStatement statement)
        {
            if (inLoop == 0)
            {
                Interpreter.ResolutionError(statement.Keyword, "No enclosing loop out of which to continue.");
            }

            return null;
        }

        public object VisitCallExpression(CallExpression expression)
        {
            Resolve(expression.Callee);

            foreach (var argument in expression.Arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        public object VisitContinueStatement(ContinueStatement statement)
        {
            if (inLoop == 0)
            {
                Interpreter.ResolutionError(statement.Keyword, "No enclosing loop out of which to continue.");
            }

            return null;
        }

        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            Resolve(statement.Expression);

            return null;
        }

        public object VisitGroupingExpression(GroupingExpression expression)
        {
            Resolve(expression.Expression);

            return null;
        }

        public object VisitIfStatement(IfStatement statement)
        {
            Resolve(statement.Condition);
            Resolve(statement.ThenBranch);
            Resolve(statement.ElseBranch);

            return null;
        }

        public object VisitLiteralExpression(LiteralExpression expression)
        {
            return null;
        }

        public object VisitLogicalExpression(LogicalExpression expression)
        {
            Resolve(expression.Left);
            Resolve(expression.Right);

            return null;
        }

        public object VisitLoopStatement(LoopStatement statement)
        {
            inLoop++;
            Resolve(statement.Initializer);
            Resolve(statement.Condition);
            Resolve(statement.Increment);
            Resolve(statement.Body);
            inLoop--;
            return null;
        }

        public object VisitReturnStatement(ReturnStatement statement)
        {
            if (currentFunction == FunctionType.None)
            {
                Interpreter.ResolutionError(statement.Keyword, "Cannot return from top-level code.");
            }

            Resolve(statement.Value);

            return null;
        }

        public object VisitUnaryExpression(UnaryExpression expression)
        {
            Resolve(expression.Right);

            return null;
        }
        #endregion
    }
}
