using Lox.Parsing;
using System.Collections.Generic;

namespace Lox.Runtime
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
                scope.Initialize(param);
            }
            Resolve(function.Body);
            scope.ExitFunction();
        }
        #endregion

        #region Statements
        public object VisitBlockStatement(BlockStatement statement)
        {
            scope.EnterBlock();
            Resolve(statement.Statements);
            scope.ExitBlock();

            return null;
        }

        public object VisitClassStatement(ClassStatement statement)
        {
            scope.Initialize(statement.Name);

            var classType = Scope.ClassType.Class;

            if (statement.Superclass != null)
            {
                classType = Scope.ClassType.Subclass;

                if (statement.Name.Lexeme == statement.Superclass.Name.Lexeme)
                {
                    Interpreter.AstError(statement.Superclass.Name, "A class cannot inherit from itself.");
                }

                Resolve(statement.Superclass);

                scope.EnterSuperclass();
            }

            scope.EnterClass(classType);
            foreach (var method in statement.Methods)
            {
                var type = LoxClass.IsInitializer(method) ? Scope.FunctionType.Initializer : Scope.FunctionType.Method;
                ResolveFunction(method, type);
            }
            scope.ExitClass();

            if (statement.Superclass != null)
            {
                scope.ExitSuperclass();
            }

            return null;
        }

        public object VisitFunctionStatement(FunctionStatement statement)
        {
            scope.Initialize(statement.Name);

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
                Interpreter.AstError(expression.Name, "Cannot read local variable in its own initializer.");
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
                Interpreter.AstError(statement.Keyword, "No enclosing loop out of which to break.");
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
                Interpreter.AstError(statement.Keyword, "No enclosing loop out of which to continue.");
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
                Interpreter.AstError(statement.Keyword, "Cannot return from top-level code.");
            }

            if (scope.InInitializer && statement.Value != null)
            {
                Interpreter.AstError(statement.Keyword, "Cannot return a value from an initializer.");
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

        public object VisitSuperExpression(SuperExpression expression)
        {
            if (!scope.InClass)
            {
                Interpreter.AstError(expression.Keyword, "Cannot use 'super' outside of a class.");
            }
            else if (!scope.InSubclass)
            {
                Interpreter.AstError(expression.Keyword, "Cannot use 'super' in a class with no superclass.");
            }

            scope.ResolveValue(expression, expression.Keyword);
            return null;
        }

        public object VisitThisExpression(ThisExpression expression)
        {
            if (scope.InClass)
            {
                scope.ResolveValue(expression, expression.Keyword);
            }
            else
            {
                Interpreter.AstError(expression.Keyword, "Cannot use 'this' outside of a class.");
            }

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
