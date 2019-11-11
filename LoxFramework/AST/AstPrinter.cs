using System.Text;

namespace LoxFramework.AST
{
    public class AstPrinter : IVisitor<string>
    {
        public string Print(Expression expression)
        {
            return expression.Accept(this);
        }

        public string VisitBinaryExpression(BinaryExpression expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Left, expression.Right);
        }

        public string VisitGroupingExpression(GroupingExpression expression)
        {
            return Parenthesize("group", expression.Expression);
        }

        public string VisitLiteralExpression(LiteralExpression expression)
        {
            return expression.Value?.ToString() ?? "nil";
        }

        public string VisitUnaryExpression(UnaryExpression expression)
        {
            return Parenthesize(expression.Operator.Lexeme, expression.Right);
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
