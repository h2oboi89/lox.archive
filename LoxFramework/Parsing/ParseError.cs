using LoxFramework.Scanning;

namespace LoxFramework.Parsing
{
    public class ParseError
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
