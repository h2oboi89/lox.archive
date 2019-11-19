using LoxFramework.AST;
using LoxFramework.Scanning;
using System;
using System.Collections.Generic;

namespace LoxFramework.Evaluating
{
    class AstInterpreter : IExpressionVisitor<object>, IStatementVisitor<object>
    {
        private Environment environment = new Environment();

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
            statement.Accept(this);
        }

        private void ExecuteBlock(IEnumerable<Statement> statements, Environment environment)
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
            return expression.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(bool))
            {
                return (bool)obj;
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
            if (operand.GetType() == typeof(double)) return;

            throw new LoxRunTimeException(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if (left.GetType() == typeof(double) && right.GetType() == typeof(double)) return;

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
                    if (left.GetType() == typeof(double) && right.GetType() == typeof(double))
                    {
                        return (double)left + (double)right;
                    }

                    if (left.GetType() == typeof(string) && right.GetType() == typeof(string))
                    {
                        return (string)left + (string)right;
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

        public object VisitGroupingExpression(GroupingExpression expression)
        {
            return Evaluate(expression.Expression);
        }

        public object VisitLiteralExpression(LiteralExpression expression)
        {
            return expression.Value;
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
        public object VisitExpressionStatement(ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
            return null;
        }

        private static string Stringify(object obj)
        {
            if (obj == null) return "nil";

            return obj.ToString();
        }

        public object VisitPrintStatement(PrintStatement statement)
        {
            var value = Evaluate(statement.Expression);
            Out?.Invoke(this, new InterpreterEventArgs(Stringify(value)));
            return null;
        }

        public object VisitVariableStatement(VariableStatement statement)
        {
            object value = null;
            if (statement.Initializer != null)
            {
                value = Evaluate(statement.Initializer);
            }

            environment.Define(statement.Name.Lexeme, value);
            return null;
        }

        public object VisitBlockStatement(BlockStatement statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment));
            return null;
        }
        #endregion
    }
}
