using LoxFramework.AST;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.LoxFramework
{
    class AstPrinter : IExpressionVisitor<string>, IStatementVisitor<string>
    {
        public string Print(Statement statement)
        {
            return statement.Accept(this);
        }

        public string VisitAssignmentExpression(AssignmentExpression expression)
        {
            return Parenthesize(expression.Name.Lexeme, expression.Value);
        }

        public string VisitBinaryExpression(BinaryExpression expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
        }

        public string VisitBlockStatement(BlockStatement statement)
        {
            return Parenthesize("block", statement.Statements);
        }

        public string VisitExpressionStatement(ExpressionStatement statement)
        {
            return statement.Expression.Accept(this);
        }

        public string VisitGroupingExpression(GroupingExpression expression)
        {
            return Parenthesize("group", expression.Expression);
        }

        public string VisitLiteralExpression(LiteralExpression expression)
        {
            return expression.Value?.ToString() ?? "nil";
        }

        public string VisitPrintStatement(PrintStatement statement)
        {
            return Parenthesize("print", statement.Expression);
        }

        public string VisitUnaryExpression(UnaryExpression expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Right);
        }

        public string VisitVariableExpression(VariableExpression expression)
        {
            return expression.Name.Lexeme;
        }

        public string VisitVariableStatement(VariableStatement statement)
        {
            return Parenthesize(statement.Name.Lexeme, statement.Initializer);
        }

        private string Parenthesize(string name, IEnumerable<Statement> statements)
        {
            var sb = new StringBuilder();

            sb.Append($"({name}");

            foreach (var statement in statements)
            {
                sb.Append($" {statement.Accept(this)}");
            }
            sb.Append(")");

            return sb.ToString();
        }

        private string Parenthesize(string name, params Expression[] expressions)
        {
            var sb = new StringBuilder();

            sb.Append($"({name}");

            foreach (var expression in expressions)
            {
                sb.Append($" {expression.Accept(this)}");
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}
