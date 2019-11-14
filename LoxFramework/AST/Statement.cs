// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using LoxFramework.Scanning;

namespace LoxFramework.AST
{
    public interface IStatementVisitor<T>
    {
        T VisitExpressionStatement(ExpressionStatement statement);
        T VisitPrintStatement(PrintStatement statement);
    }

    public abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }

    public class ExpressionStatement : Statement
    {
        public readonly Expression Expression;

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }

    public class PrintStatement : Statement
    {
        public readonly Expression Expression;

        public PrintStatement(Expression expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitPrintStatement(this);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
