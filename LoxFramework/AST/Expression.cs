// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.AST
{
    public interface IExpressionVisitor<T>
    {
        T VisitAssignmentExpression(AssignmentExpression expression);
        T VisitBinaryExpression(BinaryExpression expression);
        T VisitGroupingExpression(GroupingExpression expression);
        T VisitLiteralExpression(LiteralExpression expression);
        T VisitUnaryExpression(UnaryExpression expression);
        T VisitVariableExpression(VariableExpression expression);
    }

    public abstract class Expression
    {
        public abstract T Accept<T>(IExpressionVisitor<T> visitor);
    }

    public class AssignmentExpression : Expression
    {
        public readonly Token Name;
        public readonly Expression Value;

        public AssignmentExpression(Token name, Expression value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitAssignmentExpression(this);
        }
    }

    public class BinaryExpression : Expression
    {
        public readonly Expression Left;
        public readonly Token Operator;
        public readonly Expression Right;

        public BinaryExpression(Expression left, Token op, Expression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }

    public class GroupingExpression : Expression
    {
        public readonly Expression Expression;

        public GroupingExpression(Expression expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGroupingExpression(this);
        }
    }

    public class LiteralExpression : Expression
    {
        public readonly object Value;

        public LiteralExpression(object value)
        {
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpression(this);
        }
    }

    public class UnaryExpression : Expression
    {
        public readonly Token Operator;
        public readonly Expression Right;

        public UnaryExpression(Token op, Expression right)
        {
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpression(this);
        }
    }

    public class VariableExpression : Expression
    {
        public readonly Token Name;

        public VariableExpression(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitVariableExpression(this);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
