using System.Collections.Generic;

namespace LoxFramework.Scanning
{
    /// <summary>
    /// Scans source code and generates <see cref="Token"/>s for the next part of the interpretation of the source.
    /// </summary>
    public static class Scanner
    {
        private static string _source;
        private static Token _token;
        private static int _start;
        private static int _current;
        private static int _line;
        private static readonly Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "break", TokenType.BREAK },
            { "class", TokenType.CLASS },
            { "continue", TokenType.CONTINUE },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "return", TokenType.RETURN},
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };

        private static List<ScanError> errors = new List<ScanError>();

        public static IReadOnlyList<ScanError> Errors { get { return errors.AsReadOnly(); } }

        /// <summary>
        /// Scans the specified source and returns a collection of <see cref="Token"/>s.
        /// </summary>
        /// <param name="source">source code to scan.</param>
        /// <returns>Collection of <see cref="Token"/>s representing the source.</returns>
        public static IEnumerable<Token> Scan(string source)
        {
            errors.Clear();

            _source = source;
            _start = 0;
            _current = 0;
            _line = 1;

            while (!IsAtEnd())
            {
                _token = null;

                _start = _current;
                ScanToken();

                if (_token != null)
                {
                    yield return _token;
                }
            }

            yield return new Token(TokenType.EOF, "", null, _line);
        }

        private static void ScanToken()
        {
            var c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '%': AddToken(TokenType.MODULO); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/': SlashOrComment(); break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n': _line++; break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        IdentifierOrKeyword();
                    }
                    else
                    {
                        errors.Add(new ScanError(_line, $"Unexpected character '{c}'."));
                    }
                    break;
            }
        }

        private static void SlashOrComment()
        {
            if (Match('/'))
            {
                // comment goes until end of line or end of input
                while (Peek() != '\n' && !IsAtEnd())
                {
                    Advance();
                }
            }
            else
            {
                AddToken(TokenType.SLASH);
            }
        }

        private static void IdentifierOrKeyword()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            var text = _source.Extract(_start, _current);

            if (_keywords.ContainsKey(text))
            {
                AddToken(_keywords[text]);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER);
            }
        }

        private static bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private static bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private static void Number()
        {
            ConsumeDigits();

            // check for decimal
            if (Peek() == '.' && IsDigit(Peek(1)))
            {
                Advance();

                ConsumeDigits();
            }

            AddToken(TokenType.NUMBER, double.Parse(_source.Extract(_start, _current)));
        }

        private static void ConsumeDigits()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsAtEnd(int n = 0)
        {
            return _current + n >= _source.Length;
        }

        private static char Peek(int n = 0)
        {
            if (IsAtEnd(n)) return '\0';
            return _source[_current + n];
        }

        private static void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                {
                    _line++;
                }
                Advance();
            }

            if (IsAtEnd())
            {
                errors.Add(new ScanError(_line, "Unterminated string."));
                return;
            }

            // closing quote
            Advance();

            AddToken(TokenType.STRING, _source.Extract(_start + 1, _current - 1));
        }

        private static char Advance()
        {
            return _source[_current++];
        }

        private static void AddToken(TokenType type, object literal = null)
        {
            var text = _source.Extract(_start, _current);
            _token = new Token(type, text, literal, _line);
        }

        private static bool Match(char expected)
        {
            if (IsAtEnd() || _source[_current] != expected)
            {
                return false;
            }
            else
            {
                _current++;
                return true;
            }
        }
    }
}
