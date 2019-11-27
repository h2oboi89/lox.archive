using LoxFramework.AST;
using LoxFramework.Evaluating;
using System.Collections.Generic;

namespace LoxFramework.StaticAnalysis
{
    class Resolver : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly Scope scope;

        private Resolver(AstInterpreter interpreter)
        {
            scope = new Scope(interpreter);
        }

        public static void Resolve(AstInterpreter interpreter, IEnumerable<Statement> statements)
        {
            var resolver = new Resolver(interpreter);

            resolver.Resolve(statements);
        }

        #region Utility Methods
        internal void Resolve(IEnumerable<Statement> statements)
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

        private void ResolveFunction(FunctionStatement function, Scope.FunctionType type)
        {
            scope.EnterFunction(type);
            foreach (var param in function.Parameters)
            {
                scope.Declare(param);
                scope.Define(param);
            }
            Resolve(function.Body);
            scope.ExitFunction();
        }
        #endregion

        #region Statements
        public object VisitBlockStatement(BlockStatement statement)
        {
            scope.Enter();
            Resolve(statement.Statements);
            scope.Exit();

            return null;
        }

        public object VisitClassStatement(ClassStatement statement)
        {
            scope.Declare(statement.Name);
            scope.Define(statement.Name);

            foreach (var method in statement.Methods)
            {
                ResolveFunction(method, Scope.FunctionType.Method);
            }

            return null;
        }

        public object VisitFunctionStatement(FunctionStatement statement)
        {
            scope.Declare(statement.Name);
            scope.Define(statement.Name);

            ResolveFunction(statement, Scope.FunctionType.Function);

            return null;
        }

        public object VisitVariableStatement(VariableStatement statement)
        {
            scope.Declare(statement.Name);
            Resolve(statement.Initializer);
            scope.Define(statement.Name);

            return null;
        }

        #endregion

        #region Expressions
        public object VisitAssignmentExpression(AssignmentExpression expression)
        {
            Resolve(expression.Value);
            scope.ResolveValue(expression, expression.Name);

            return null;
        }

        public object VisitVariableExpression(VariableExpression expression)
        {
            if (scope.IsDeclared(expression.Name) && !scope.IsDefined(expression.Name))
            {
                Interpreter.ScopeError(expression.Name, "Cannot read local variable in its own initializer.");
            }

            scope.ResolveValue(expression, expression.Name);

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
            if (!scope.InLoop)
            {
                Interpreter.ScopeError(statement.Keyword, "No enclosing loop out of which to break.");
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
            if (!scope.InLoop)
            {
                Interpreter.ScopeError(statement.Keyword, "No enclosing loop out of which to continue.");
            }

            return null;
        }

        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            Resolve(statement.Expression);

            return null;
        }

        public object VisitGetExpression(GetExpression expression)
        {
            Resolve(expression.Obj);

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
            scope.EnterLoop();
            Resolve(statement.Initializer);
            Resolve(statement.Condition);
            Resolve(statement.Increment);
            Resolve(statement.Body);
            scope.ExitLoop();

            return null;
        }

        public object VisitReturnStatement(ReturnStatement statement)
        {
            if (!scope.InFunction)
            {
                Interpreter.ScopeError(statement.Keyword, "Cannot return from top-level code.");
            }

            Resolve(statement.Value);

            return null;
        }

        public object VisitSetExpression(SetExpression expression)
        {
            Resolve(expression.Value);
            Resolve(expression.Obj);

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
