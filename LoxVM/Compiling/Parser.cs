using Scanning;
using System.Collections.Generic;

namespace LoxVM.Compiling
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
                compilingChunk.AddOpCode(OpCode.RETURN, 1);
                return compilingChunk;
            }

            Expression();

            compilingChunk.AddOpCode(OpCode.RETURN, PreviousToken.Line);

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

        private static void ParsePrecedence(Precedence precedence)
        {
            Advance();
            var prefixRule = ParseRule.GetRule(PreviousToken.Type).Prefix;

            if (prefixRule == null)
            {
                Compiler.ParseError(PreviousToken, "Expect expression.");
                return;
            }

            prefixRule();

            while (precedence <= ParseRule.GetRule(Peek().Type).Precedence)
            {
                Advance();

                var infixRule = ParseRule.GetRule(PreviousToken.Type).Infix;

                infixRule();
            }
        }
        #endregion

        #region Grammar Rules
        private static void Expression()
        {
            ParsePrecedence(Precedence.ASSIGNMENT);
        }

        internal static void Grouping()
        {
            Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
        }

        internal static void Unary()
        {
            var op = PreviousToken;

            ParsePrecedence(Precedence.UNARY);

            switch (op.Type)
            {
                case TokenType.BANG: compilingChunk.AddOpCode(OpCode.NOT, op.Line); break;
                case TokenType.MINUS: compilingChunk.AddOpCode(OpCode.NEGATE, op.Line); break;
            }
        }

        internal static void Binary()
        {
            var op = PreviousToken;

            var rule = ParseRule.GetRule(op.Type);
            ParsePrecedence(rule.Precedence + 1);

            switch (op.Type)
            {
                case TokenType.PLUS: compilingChunk.AddOpCode(OpCode.ADD, op.Line); break;
                case TokenType.MINUS: compilingChunk.AddOpCode(OpCode.SUBTRACT, op.Line); break;
                case TokenType.STAR: compilingChunk.AddOpCode(OpCode.MULTIPLY, op.Line); break;
                case TokenType.SLASH: compilingChunk.AddOpCode(OpCode.DIVIDE, op.Line); break;
            }
        }

        internal static void Number()
        {
            compilingChunk.AddConstant((double)PreviousToken.Literal, PreviousToken.Line);
        }

        internal static void Literal()
        {
            switch (PreviousToken.Type)
            {
                case TokenType.FALSE: compilingChunk.AddOpCode(OpCode.FALSE, PreviousToken.Line); break;
                case TokenType.TRUE: compilingChunk.AddOpCode(OpCode.TRUE, PreviousToken.Line); break;
                case TokenType.NIL: compilingChunk.AddOpCode(OpCode.NIL, PreviousToken.Line); break;
            }
        }
        #endregion
    }
}
