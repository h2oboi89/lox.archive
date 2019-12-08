using Scanning;
using System;
using System.Collections.Generic;

namespace Lox.Parsing
{
    class Parser
    {
        private const byte MAX_ARGUMENT_COUNT = byte.MaxValue;

        private readonly List<Token> tokens;
        private int current = 0;

        private static readonly List<ParseError> errors = new List<ParseError>();

        public static IReadOnlyList<ParseError> Errors { get { return errors.AsReadOnly(); } }

        private Parser(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
        }

        public static IEnumerable<Statement> Parse(IEnumerable<Token> tokens)
        {
            errors.Clear();

            return new Parser(tokens).Parse();
        }

        private IEnumerable<Statement> Parse()
        {
            var statements = new List<Statement>();

            while (!IsAtEnd)
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
            if (IsAtEnd) return false;
            return Peek().Type == tokenType;
        }

        private Token Advance()
        {
            if (!IsAtEnd) current++;
            return PreviousToken;
        }

        private bool IsAtEnd { get { return Peek().Type == TokenType.EOF; } }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token PreviousToken { get { return tokens[current - 1]; } }

        private Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType)) return Advance();

            throw Error(Peek(), message);
        }

        private ParseException Error(Token token, string message)
        {
            errors.Add(new ParseError(token, message));
            return new ParseException();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd)
            {
                if (PreviousToken.Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
        #endregion

        #region Grammar Rules
#pragma warning disable CA1031 // Do not catch general exception types
        private Statement Declaration()
        {
            try
            {
                if (Match(TokenType.CLASS)) return ClassDeclaration();
                if (Match(TokenType.FUN)) return FunctionDeclaration("function");
                if (Match(TokenType.VAR)) return VariableDeclaration();

                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types

        private Statement ClassDeclaration()
        {
            var name = Consume(TokenType.IDENTIFIER, "Expect class name.");

            VariableExpression superclass = null;
            if (Match(TokenType.LESS))
            {
                Consume(TokenType.IDENTIFIER, "Expect superclass name.");
                superclass = new VariableExpression(PreviousToken);
            }

            Consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");

            var methods = new List<FunctionStatement>();
            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
            {
                methods.Add(FunctionDeclaration("method"));
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");

            return new ClassStatement(name, superclass, methods);
        }

        private FunctionStatement FunctionDeclaration(string kind)
        {
            var name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");

            Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");

            var parameters = new List<Token>();

            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= MAX_ARGUMENT_COUNT)
                    {
                        Error(Peek(), $"Cannot have more than {MAX_ARGUMENT_COUNT} parameters.");
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));
            }

            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            Consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");

            var body = Block();

            return new FunctionStatement(name, parameters, body);
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
            if (Match(TokenType.CONTINUE)) return ContinueStatement();
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStatement(Block());

            return ExpressionStatement();
        }

        private Statement BreakStatement()
        {
            var keyword = PreviousToken;

            Consume(TokenType.SEMICOLON, "Expect ';' after 'break'.");

            return new BreakStatement(keyword);
        }

        private Statement ContinueStatement()
        {
            var keyword = PreviousToken;

            Consume(TokenType.SEMICOLON, "Expect ';' after 'continue'.");

            return new ContinueStatement(keyword);
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

            var body = Statement();

            return new LoopStatement(initializer, condition, increment, body);
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

        private Statement ReturnStatement()
        {
            var keyword = PreviousToken;
            Expression value = null;

            if (!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after a return value.");

            return new ReturnStatement(keyword, value);
        }

        private Statement WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            var condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

            var body = Statement();

            return new LoopStatement(null, condition, null, body);
        }

        private IEnumerable<Statement> Block()
        {
            var statements = new List<Statement>();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd)
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
                var equals = PreviousToken;
                var value = Assignment();

                if (expression is VariableExpression variableExpression)
                {
                    return new AssignmentExpression(variableExpression.Name, value);
                }
                else if (expression is GetExpression getExpression)
                {
                    return new SetExpression(getExpression.Obj, getExpression.Name, value);
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
                var op = PreviousToken;
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
                var op = PreviousToken;
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
                var op = PreviousToken;
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
                var op = PreviousToken;
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
                var op = PreviousToken;
                var right = Multiplication();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Multiplication()
        {
            var expression = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR, TokenType.MODULO))
            {
                var op = PreviousToken;
                var right = Unary();
                expression = new BinaryExpression(expression, op, right);
            }

            return expression;
        }

        private Expression Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                var op = PreviousToken;
                var right = Unary();
                return new UnaryExpression(op, right);
            }

            return Call();
        }

        private Expression Call()
        {
            var expression = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expression = FinishCall(expression);
                }
                else if (Match(TokenType.DOT))
                {
                    var name = Consume(TokenType.IDENTIFIER, "Expect property name after '.'.");

                    expression = new GetExpression(expression, name);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        private Expression FinishCall(Expression callee)
        {
            var arguments = new List<Expression>();

            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= MAX_ARGUMENT_COUNT)
                    {
                        Error(Peek(), $"Cannot have more than {MAX_ARGUMENT_COUNT} arguments.");
                    }
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }

            var paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");

            return new CallExpression(callee, paren, arguments);
        }

        private Expression Primary()
        {
            if (Match(TokenType.FALSE)) return new LiteralExpression(false);
            if (Match(TokenType.TRUE)) return new LiteralExpression(true);
            if (Match(TokenType.NIL)) return new LiteralExpression(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new LiteralExpression(PreviousToken.Literal);
            }

            if (Match(TokenType.SUPER))
            {
                var keyword = PreviousToken;
                Consume(TokenType.DOT, "Expect '.' after 'super'.");
                var method = Consume(TokenType.IDENTIFIER, "Expect superclass method name.");
                return new SuperExpression(keyword, method);
            }

            if (Match(TokenType.THIS))
            {
                return new ThisExpression(PreviousToken);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new VariableExpression(PreviousToken);
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                var expression = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupingExpression(expression);
            }

            throw Error(Peek(), "Expect expression.");
        }
        #endregion

#pragma warning disable CA1032 // Implement standard exception constructors
        private class ParseException : Exception
        {
            public ParseException() { }
        }
#pragma warning restore CA1032 // Implement standard exception constructors
    }
}
