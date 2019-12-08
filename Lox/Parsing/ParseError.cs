using Scanning;

namespace Lox.Parsing
{
    class ParseError
    {
        public Token Token { get; private set; }

        public string Message { get; private set; }

        public ParseError(Token token, string message)
        {
            Token = token;
            Message = message;
        }
    }
}
