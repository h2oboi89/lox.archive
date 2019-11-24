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
        private bool inLoop = false;

        private Parser(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
        }

        public static IEnumerable<Statement> Parse(IEnumerable<Token> tokens)
        {
            return new Parser(tokens).Parse();
        }

        private IEnumerable<Statement> Parse()
        {
            var statements = new List<Statement>();

            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
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

        private Statement LoopBody()
        {
            inLoop = true;
            var body = Statement();
            inLoop = false;

            return body;
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
        private Statement Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VariableDeclaration();

                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }

        private Statement VariableDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expression intializer = null;

            if (Match(TokenType.EQUAL))
            {
                intializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");

            return new VariableStatement(name, intializer);
        }

        private Statement Statement()
        {
            if (Match(TokenType.BREAK)) return BreakStatement();
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStatement(Block());

            return ExpressionStatement();
        }

        private Statement BreakStatement()
        {
            if (!inLoop) throw Error(Previous(), "No enclosing loop out of which to break.");

            Consume(TokenType.SEMICOLON, "Expect ';' after 'break'.");

            return new BreakStatement();
        }

        private Statement ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

            Statement initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VariableDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            Expression condition;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            else
            {
                condition = new LiteralExpression(true);
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expression increment = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            var body = LoopBody();

            return new ForStatement(initializer, condition, increment, body);
        }

        private Statement IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

            var thenBranch = Statement();
            Statement elseBranch = null;

            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new IfStatement(condition, thenBranch, elseBranch);
        }

        private Statement PrintStatement()
        {
            var value = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after value.");

            return new PrintStatement(value);
        }

        private Statement WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

            var body = LoopBody();

            return new WhileStatement(condition, body);
        }

        private IEnumerable<Statement> Block()
        {
            var statements = new List<Statement>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Statement ExpressionStatement()
        {
            var expression = Expression();

            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");

            return new ExpressionStatement(expression);
        }

        private Expression Expression()
        {
            return Assignment();
        }

        private Expression Assignment()
        {
            var expression = Or();

            if (Match(TokenType.EQUAL))
            {
                var equals = Previous();
                var value = Assignment();

                if (expression.GetType() == typeof(VariableExpression))
                {
                    var name = ((VariableExpression)expression).Name;
                    return new AssignmentExpression(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expression;
        }

        private Expression Or()
        {
            var expression = And();

            while (Match(TokenType.OR))
            {
                var op = Previous();
                var right = And();
                expression = new LogicalExpression(expression, op, right);
            }

            return expression;
        }

        private Expression And()
        {
            var expression = Equality();

            while (Match(TokenType.AND))
            {
                var op = Previous();
                var right = Equality();
                expression = new LogicalExpression(expression, op, right);
            }

            return expression;
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

            if (Match(TokenType.IDENTIFIER))
            {
                return new VariableExpression(Previous());
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
