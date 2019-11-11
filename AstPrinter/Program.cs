using LoxFramework.AST;
using LoxFramework.Scanning;
using System;
using System.Diagnostics;
using System.Text;

namespace AstPrinter
{
    class Program
    {
        class AstPrinter : IVisitor<string>
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
                if (expression == null)
                {
                    return "nil";
                }
                return expression.Value.ToString();
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

        static void Main()
        {
            var expression = new BinaryExpression(
                new UnaryExpression(
                    new Token(TokenType.MINUS, "-", null, 1),
                    new LiteralExpression(123)
                    ),
                new Token(TokenType.STAR, "*", null, 1),
                new GroupingExpression(
                    new LiteralExpression(45.67)
                    )
                );

            Console.WriteLine(new AstPrinter().Print(expression));

            if (Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }
        }
    }
}
