// Generated code, do not modify.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Scanning;
using System;

namespace LoxVM.Compiling
{
    class ParseRule
    {
        public Action Prefix { get; private set; }
        public Action Infix { get; private set; }
        public Precedence Precedence { get; private set; }

        private ParseRule(Action prefix, Action infix, Precedence precedence)
        {
            Prefix = prefix;
            Infix = infix;
            Precedence = precedence;
        }

        public static ParseRule GetRule(TokenType tokenType)
        {
            return rules[(int)tokenType];
        }

        private static readonly ParseRule[] rules = new ParseRule[]
        {
            new ParseRule(Parser.Grouping, null, Precedence.NONE),           // TokenType.LEFT_PAREN
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.RIGHT_PAREN
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.LEFT_BRACE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.RIGHT_BRACE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.COMMA
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.DOT
            new ParseRule(Parser.Unary, Parser.Binary, Precedence.TERM),     // TokenType.MINUS
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.MODULO
            new ParseRule(null, Parser.Binary, Precedence.TERM),             // TokenType.PLUS
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.SEMICOLON
            new ParseRule(null, Parser.Binary, Precedence.FACTOR),           // TokenType.SLASH
            new ParseRule(null, Parser.Binary, Precedence.FACTOR),           // TokenType.STAR
            new ParseRule(Parser.Unary, null, Precedence.NONE),              // TokenType.BANG
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.BANG_EQUAL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.EQUAL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.EQUAL_EQUAL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.GREATER
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.GREATER_EQUAL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.LESS
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.LESS_EQUAL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.IDENTIFIER
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.STRING
            new ParseRule(Parser.Number, null, Precedence.NONE),             // TokenType.NUMBER
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.AND
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.BREAK
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.CLASS
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.CONTINUE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.ELSE
            new ParseRule(Parser.Literal, null, Precedence.NONE),            // TokenType.FALSE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.FUN
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.FOR
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.IF
            new ParseRule(Parser.Literal, null, Precedence.NONE),            // TokenType.NIL
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.OR
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.RETURN
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.SUPER
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.THIS
            new ParseRule(Parser.Literal, null, Precedence.NONE),            // TokenType.TRUE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.VAR
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.WHILE
            new ParseRule(null, null, Precedence.NONE),                      // TokenType.EOF
        };
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
