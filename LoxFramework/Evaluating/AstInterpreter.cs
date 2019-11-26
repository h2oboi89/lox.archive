using LoxFramework.AST;
using LoxFramework.Evaluating.Globals;
using LoxFramework.Scanning;
using System;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class AstInterpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private readonly bool interactive;
        public Environment Globals { get; private set; }
        private Environment environment;

        public AstInterpreter(bool interactive)
        {
            this.interactive = interactive;
            Reset(interactive);
        }

        private Token GlobalName(string name)
        {
            return new Token(TokenType.FUN, name, null, -1);
        }

        private void SetupGlobals(bool interactive)
        {
            Globals = new Environment(interactive: interactive);

            Globals.Define(GlobalName("clock"), new Clock());
        }

        public void Reset(bool interactive)
        {
            SetupGlobals(interactive);

            environment = Globals;
        }

        public void Interpret(IEnumerable<Statement> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (LoxRunTimeException e)
            {
                Interpreter.InterpretError(e);
            }
        }

        private void Execute(Statement statement)
        {
            statement?.Accept(this);
        }

        internal void ExecuteBlock(IEnumerable<Statement> statements, Environment environment)
        {
            var enclosingEnvironment = this.environment;

            try
            {
                this.environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            finally
            {
                this.environment = enclosingEnvironment;
            }
        }

        public event EventHandler<InterpreterEventArgs> Out;

        #region Expressions
        private object Evaluate(Expression expression)
        {
            return expression?.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is bool bObj)
            {
                return bObj;
            }

            return true;
        }

        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null)
            {
                return true;
            }

            if (a == null)
            {
                return false;
            }

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if (operand is double) return;

            throw new LoxRunTimeException(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left is double && right is double) return;

            throw new LoxRunTimeException(op, "Operands must be numbers.");
        }

        public object VisitBinaryExpression(BinaryExpression expression)
        {
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                // Comparisons
                case TokenType.GREATER:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                // Arithmetic
                case TokenType.MINUS:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double dLeft && right is double dRight)
                    {
                        return dLeft + dRight;
                    }

                    if (left is string sLeft && right is string sRight)
                    {
                        return sLeft + sRight;
                    }

                    throw new LoxRunTimeException(expression.Operator, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expression.Operator, left, right);
                    return (double)left * (double)right;
            }

            // unreachable
            return null;
        }

        public object VisitCallExpression(CallExpression expression)
        {
            var callee = Evaluate(expression.Callee);

            var arguments = new List<object>();

            foreach (var argument in expression.Arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ILoxCallable function))
            {
                throw new LoxRunTimeException(expression.Paren, "Can only call functions and classes.");
            }

            if (arguments.Count != function.Arity())
            {
                throw new LoxRunTimeException(expression.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }

        public object VisitGroupingExpression(GroupingExpression expression)
        {
            return Evaluate(expression.Expression);
        }

        public object VisitLiteralExpression(LiteralExpression expression)
        {
            return expression.Value;
        }

        public object VisitLogicalExpression(LogicalExpression expression)
        {
            var left = Evaluate(expression.Left);

            if (expression.Operator.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expression.Right);
        }

        public object VisitUnaryExpression(UnaryExpression expression)
        {
            var right = Evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperand(expression.Operator, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !IsTruthy(right);
            }

            // unreachable
            return null;
        }

        public object VisitVariableExpression(VariableExpression expression)
        {
            return environment.Get(expression.Name);
        }

        public object VisitAssignmentExpression(AssignmentExpression expression)
        {
            var value = Evaluate(expression.Value);

            environment.Assign(expression.Name, value);

            return value;
        }
        #endregion

        #region Statements
        private static string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is bool bObj)
            {
                return bObj ? "true" : "false";
            }

            return obj.ToString();
        }

        public object VisitBlockStatement(BlockStatement statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment, interactive));

            return null;
        }

        public object VisitBreakStatement(BreakStatement statement)
        {
            throw new LoxBreakException();
        }

        public object VisitContinueStatement(ContinueStatement statement)
        {
            throw new LoxContinueException();
        }

        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            var value = Evaluate(statement.Expression);

            Out?.Invoke(this, new InterpreterEventArgs(Stringify(value), true));

            return null;
        }

        public object VisitFunctionStatement(FunctionStatement statement)
        {
            var function = new LoxFunction(statement);

            environment.Define(statement.Name, function);

            return null;
        }

        public object VisitIfStatement(IfStatement statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                Execute(statement.ThenBranch);
            }
            else if (statement.ElseBranch != null)
            {
                Execute(statement.ElseBranch);
            }

            return null;
        }

        public object VisitLoopStatement(LoopStatement statement)
        {
            Execute(statement.Initializer);

            while (IsTruthy(Evaluate(statement.Condition)))
            {
                try
                {
                    Execute(statement.Body);
                }
                catch (LoxBreakException) { break; }
                catch (LoxContinueException) { continue; }
                finally
                {
                    Evaluate(statement.Increment);
                }
            }

            return null;
        }

        public object VisitPrintStatement(PrintStatement statement)
        {
            var value = Evaluate(statement.Expression);

            Out?.Invoke(this, new InterpreterEventArgs(Stringify(value)));

            return null;
        }

        public object VisitReturnStatement(ReturnStatement statement)
        {
            var value = Evaluate(statement.Value);

            throw new LoxReturn(value);
        }

        public object VisitVariableStatement(VariableStatement statement)
        {
            var value = Evaluate(statement.Initializer);

            environment.Define(statement.Name, value);

            return null;
        }
        #endregion
    }
}
