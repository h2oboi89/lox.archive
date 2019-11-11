using LoxFramework.AST;
using LoxFramework.Scanning;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LoxFramework.Parsing
{
    class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
        }

        public Expression Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseException)
            {
                return null;
            }
        }

        #region Utility Methods
        private bool Match(params TokenType[] tokenTypes)
        {
            foreach (var tokenType in tokenTypes)
            {
                if (Check(tokenType))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private bool Check(TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message)
        {
            Interpreter.ParseError(token, message);
            return new ParseException();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
        #endregion

        #region Grammer Rules
        private Expression Expression()
        {
            return Equality();
        }

        private Expression Equality()
        {
            var expression = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Comparison()
        {
            var expression = Addition();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var op = Previous();
                var right = Addition();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Addition()
        {
            var expression = Multiplication();

            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                var op = Previous();
                var right = Multiplication();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Multiplication()
        {
            var expression = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                var op = Previous();
                var right = Unary();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new UnaryExpression(op, right);
            }

            return Primary();
        }

        private Expression Primary()
        {
            if (Match(TokenType.FALSE)) return new LiteralExpression(false);
            if (Match(TokenType.TRUE)) return new LiteralExpression(true);
            if (Match(TokenType.NIL)) return new LiteralExpression(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new LiteralExpression(Previous().Literal);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expression = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupingExpression(expression);
            }

            throw Error(Peek(), "Expect expression.");
        }

        [Serializable]
        private class ParseException : Exception
        {
            public ParseException() { }

            public ParseException(string message) : base(message) { }

            public ParseException(string message, Exception innerException) : base(message, innerException) { }

            protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        #endregion
    }
}
