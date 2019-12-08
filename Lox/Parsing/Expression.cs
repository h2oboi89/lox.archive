// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Scanning;
using System.Collections.Generic;

namespace Lox.Parsing
{
    public interface IExpressionVisitor<T>
    {
        T VisitAssignmentExpression(AssignmentExpression expression);
        T VisitBinaryExpression(BinaryExpression expression);
        T VisitCallExpression(CallExpression expression);
        T VisitGetExpression(GetExpression expression);
        T VisitGroupingExpression(GroupingExpression expression);
        T VisitLiteralExpression(LiteralExpression expression);
        T VisitLogicalExpression(LogicalExpression expression);
        T VisitSetExpression(SetExpression expression);
        T VisitSuperExpression(SuperExpression expression);
        T VisitThisExpression(ThisExpression expression);
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
        public readonly Token Op;
        public readonly Expression Right;

        public BinaryExpression(Expression left, Token op, Expression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpression(this);
        }
    }

    public class CallExpression : Expression
    {
        public readonly Expression Callee;
        public readonly Token Paren;
        public readonly IEnumerable<Expression> Arguments;

        public CallExpression(Expression callee, Token paren, IEnumerable<Expression> arguments)
        {
            Callee = callee;
            Paren = paren;
            Arguments = arguments;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitCallExpression(this);
        }
    }

    public class GetExpression : Expression
    {
        public readonly Expression Obj;
        public readonly Token Name;

        public GetExpression(Expression obj, Token name)
        {
            Obj = obj;
            Name = name;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitGetExpression(this);
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

    public class LogicalExpression : Expression
    {
        public readonly Expression Left;
        public readonly Token Op;
        public readonly Expression Right;

        public LogicalExpression(Expression left, Token op, Expression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitLogicalExpression(this);
        }
    }

    public class SetExpression : Expression
    {
        public readonly Expression Obj;
        public readonly Token Name;
        public readonly Expression Value;

        public SetExpression(Expression obj, Token name, Expression value)
        {
            Obj = obj;
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitSetExpression(this);
        }
    }

    public class SuperExpression : Expression
    {
        public readonly Token Keyword;
        public readonly Token Method;

        public SuperExpression(Token keyword, Token method)
        {
            Keyword = keyword;
            Method = method;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitSuperExpression(this);
        }
    }

    public class ThisExpression : Expression
    {
        public readonly Token Keyword;

        public ThisExpression(Token keyword)
        {
            Keyword = keyword;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.VisitThisExpression(this);
        }
    }

    public class UnaryExpression : Expression
    {
        public readonly Token Op;
        public readonly Expression Right;

        public UnaryExpression(Token op, Expression right)
        {
            Op = op;
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
