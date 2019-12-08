using LoxFramework.Parsing;
using LoxFramework.Scanning;
using System;
using System.Collections.Generic;

namespace LoxVM
{
    class Parser
    {
        private static List<Token> tokens;
        private static Chunk compilingChunk;
        private static int current;

        private static readonly List<ParseError> errors = new List<ParseError>();

        public static IReadOnlyList<ParseError> Errors { get { return errors.AsReadOnly(); } }

        public static Chunk Parse(IEnumerable<Token> tokens)
        {
            Parser.tokens = new List<Token>(tokens);
            compilingChunk = new Chunk();
            current = 0;
            errors.Clear();

            return Parse();
        }

        private static Chunk Parse()
        {
            // check for empty input (EOF token)
            if (tokens.Count == 1)
            {
                compilingChunk.AddInstruction(OpCode.RETURN, 1);
                return compilingChunk;
            }

            Expression();

            compilingChunk.AddInstruction(OpCode.RETURN, PreviousToken.Line);

            return compilingChunk;
        }

        #region Utility Methods
        private static bool Match(params TokenType[] tokenTypes)
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

        private static bool Check(TokenType tokenType)
        {
            if (IsAtEnd) return false;
            return Peek().Type == tokenType;
        }

        private static Token Advance()
        {
            if (!IsAtEnd) current++;
            return PreviousToken;
        }

        private static bool IsAtEnd { get { return Peek().Type == TokenType.EOF; } }

        private static Token Peek()
        {
            return tokens[current];
        }

        private static Token PreviousToken { get { return tokens[current - 1]; } }

        private static Token Consume(TokenType tokenType, string message)
        {
            if (Check(tokenType)) return Advance();

            throw Error(Peek(), message);
        }

        private static ParseException Error(Token token, string message)
        {
            errors.Add(new ParseError(token, message));
            return new ParseException();
        }

        private static void Synchronize()
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
        private class ParseRule
        {
            public Action Prefix { get; private set; }
            public Action Infix { get; private set; }
            public Precedence Precedence { get; private set; }

            public ParseRule(Action prefix, Action infix, Precedence precedence)
            {
                Prefix = prefix;
                Infix = infix;
                Precedence = precedence;
            }
        }

        private static ParseRule[] rules = new ParseRule[]
        {
            new ParseRule(Grouping, null, Precedence.NONE),     // TokenType.LEFT_PAREN
            new ParseRule(null, null, Precedence.NONE),         // TokenType.RIGHT_PAREN
            new ParseRule(null, null, Precedence.NONE),         // TokenType.LEFT_BRACE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.RIGHT_BRACE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.COMMA
            new ParseRule(null, null, Precedence.NONE),         // TokenType.DOT
            new ParseRule(Unary, Binary, Precedence.TERM),      // TokenType.MINUS
            new ParseRule(null, null, Precedence.NONE),         // TokenType.MODULO
            new ParseRule(null, Binary, Precedence.TERM),       // TokenType.PLUS
            new ParseRule(null, null, Precedence.NONE),         // TokenType.SEMICOLON
            new ParseRule(null, Binary, Precedence.FACTOR),     // TokenType.SLASH
            new ParseRule(null, Binary, Precedence.FACTOR),     // TokenType.STAR

            new ParseRule(null, null, Precedence.NONE),         // TokenType.BANG
            new ParseRule(null, null, Precedence.NONE),         // TokenType.BANG_EQUAL
            new ParseRule(null, null, Precedence.NONE),         // TokenType.EQUAL
            new ParseRule(null, null, Precedence.NONE),         // TokenType.EQUAL_EQUAL
            new ParseRule(null, null, Precedence.NONE),         // TokenType.GREATER
            new ParseRule(null, null, Precedence.NONE),         // TokenType.GREATER_EQUAL
            new ParseRule(null, null, Precedence.NONE),         // TokenType.LESS
            new ParseRule(null, null, Precedence.NONE),         // TokenType.LESS_EQUAL

            new ParseRule(null, null, Precedence.NONE),         // TokenType.IDENTIFIER
            new ParseRule(null, null, Precedence.NONE),         // TokenType.STRING
            new ParseRule(Number, null, Precedence.NONE),       // TokenType.NUMBER

            new ParseRule(null, null, Precedence.NONE),         // TokenType.AND
            new ParseRule(null, null, Precedence.NONE),         // TokenType.BREAK
            new ParseRule(null, null, Precedence.NONE),         // TokenType.CLASS
            new ParseRule(null, null, Precedence.NONE),         // TokenType.CONTINUE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.ELSE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.FALSE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.FUN
            new ParseRule(null, null, Precedence.NONE),         // TokenType.FOR
            new ParseRule(null, null, Precedence.NONE),         // TokenType.IF
            new ParseRule(null, null, Precedence.NONE),         // TokenType.NIL
            new ParseRule(null, null, Precedence.NONE),         // TokenType.OR
            new ParseRule(null, null, Precedence.NONE),         // TokenType.RETURN
            new ParseRule(null, null, Precedence.NONE),         // TokenType.SUPER
            new ParseRule(null, null, Precedence.NONE),         // TokenType.THIS
            new ParseRule(null, null, Precedence.NONE),         // TokenType.TRUE
            new ParseRule(null, null, Precedence.NONE),         // TokenType.VAR
            new ParseRule(null, null, Precedence.NONE),         // TokenType.WHILE

            new ParseRule(null, null, Precedence.NONE)          // TokenType.EOF
        };

        private static ParseRule GetRule(TokenType tokenType)
        {
            return rules[(int)tokenType];
        }

        private static void ParsePrecedence(Precedence precedence)
        {
            Advance();
            var prefixRule = GetRule(PreviousToken.Type).Prefix;

            if (prefixRule == null)
            {
                Compiler.ParseError(PreviousToken, "Expect expression.");
                return;
            }

            prefixRule();

            while (precedence <= GetRule(Peek().Type).Precedence)
            {
                Advance();

                var infixRule = GetRule(PreviousToken.Type).Infix;

                infixRule();
            }
        }

        private static void Expression()
        {
            ParsePrecedence(Precedence.ASSIGNMENT);
        }

        private static void Grouping()
        {
            Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
        }

        private static void Unary()
        {
            var op = PreviousToken;

            ParsePrecedence(Precedence.UNARY);

            if (op.Type == TokenType.MINUS)
            {
                compilingChunk.AddInstruction(OpCode.NEGATE, op.Line);
            }
        }

        private static void Binary()
        {
            var op = PreviousToken;

            var rule = GetRule(op.Type);
            ParsePrecedence(rule.Precedence + 1);

            switch (op.Type)
            {
                case TokenType.PLUS: compilingChunk.AddInstruction(OpCode.ADD, op.Line); break;
                case TokenType.MINUS: compilingChunk.AddInstruction(OpCode.SUBTRACT, op.Line); break;
                case TokenType.STAR: compilingChunk.AddInstruction(OpCode.MULTIPLY, op.Line); break;
                case TokenType.SLASH: compilingChunk.AddInstruction(OpCode.DIVIDE, op.Line); break;
            }
        }

        private static void Number()
        {
            compilingChunk.AddConstant((double)PreviousToken.Literal, PreviousToken.Line);
        }
        #endregion
    }
}
