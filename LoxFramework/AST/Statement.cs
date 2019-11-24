// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using LoxFramework.Scanning;
using System.Collections.Generic;

namespace LoxFramework.AST
{
    interface IStatementVisitor<T>
    {
        T VisitBlockStatement(BlockStatement statement);
        T VisitBreakStatement(BreakStatement statement);
        T VisitExpressionStatement(ExpressionStatement statement);
        T VisitIfStatement(IfStatement statement);
        T VisitPrintStatement(PrintStatement statement);
        T VisitVariableStatement(VariableStatement statement);
        T VisitWhileStatement(WhileStatement statement);
        T VisitForStatement(ForStatement statement);
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

    class BreakStatement : Statement
    {
        public BreakStatement() { }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBreakStatement(this);
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

    class WhileStatement : Statement
    {
        public readonly Expression Condition;
        public readonly Statement Body;

        public WhileStatement(Expression condition, Statement body)
        {
            Condition = condition;
            Body = body;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }

    class ForStatement : Statement
    {
        public readonly Statement Initializer;
        public readonly Expression Condition;
        public readonly Expression Increment;
        public readonly Statement Body;

        public ForStatement(Statement initializer, Expression condition, Expression increment, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitForStatement(this);
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
