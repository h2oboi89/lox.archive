// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.AST
{
    interface IStatementVisitor<T>
    {
        T VisitBlockStatement(BlockStatement statement);
        T VisitExpressionStatement(ExpressionStatement statement);
        T VisitIfStatement(IfStatement statement);
        T VisitPrintStatement(PrintStatement statement);
        T VisitVariableStatement(VariableStatement statement);
    }

    abstract class Statement
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }

    class BlockStatement : Statement
    {
        public readonly IEnumerable<Statement> Statements;

        public BlockStatement(IEnumerable<Statement> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBlockStatement(this);
        }
    }

    class ExpressionStatement : Statement
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

    class IfStatement : Statement
    {
        public readonly Expression Condition;
        public readonly Statement ThenBranch;
        public readonly Statement ElseBranch;

        public IfStatement(Expression condition, Statement thenBranch, Statement elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }

    class PrintStatement : Statement
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

    class VariableStatement : Statement
    {
        public readonly Token Name;
        public readonly Expression Initializer;

        public VariableStatement(Token name, Expression initializer)
        {
            Name = name;
            Initializer = initializer;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitVariableStatement(this);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
