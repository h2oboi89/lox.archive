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
        T VisitContinueStatement(ContinueStatement statement);
        T VisitExpressionStatement(ExpressionStatement statement);
        T VisitFunctionStatement(FunctionStatement statement);
        T VisitIfStatement(IfStatement statement);
        T VisitLoopStatement(LoopStatement statement);
        T VisitReturnStatement(ReturnStatement statement);
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

    class BreakStatement : Statement
    {
        public readonly Token Keyword;

        public BreakStatement(Token keyword)
        {
            Keyword = keyword;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitBreakStatement(this);
        }
    }

    class ContinueStatement : Statement
    {
        public readonly Token Keyword;

        public ContinueStatement(Token keyword)
        {
            Keyword = keyword;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitContinueStatement(this);
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

    class FunctionStatement : Statement
    {
        public readonly Token Name;
        public readonly IEnumerable<Token> Parameters;
        public readonly IEnumerable<Statement> Body;

        public FunctionStatement(Token name, IEnumerable<Token> parameters, IEnumerable<Statement> body)
        {
            Name = name;
            Parameters = parameters;
            Body = body;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitFunctionStatement(this);
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

    class LoopStatement : Statement
    {
        public readonly Statement Initializer;
        public readonly Expression Condition;
        public readonly Expression Increment;
        public readonly Statement Body;

        public LoopStatement(Statement initializer, Expression condition, Expression increment, Statement body)
        {
            Initializer = initializer;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitLoopStatement(this);
        }
    }

    class ReturnStatement : Statement
    {
        public readonly Token Keyword;
        public readonly Expression Value;

        public ReturnStatement(Token keyword, Expression value)
        {
            Keyword = keyword;
            Value = value;
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
        {
            return visitor.VisitReturnStatement(this);
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
