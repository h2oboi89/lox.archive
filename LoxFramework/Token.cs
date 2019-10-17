namespace LoxFramework
{
    /// <summary>
    /// Token serves as the smallest atomic element during scanning.
    /// </summary>
    class Token
    {
        /// <summary>
        /// The grammatical type of the scanned lexeme.
        /// </summary>
        internal readonly TokenType Type;
        readonly string Lexeme;
        readonly object Literal;
        readonly int Line;

        /// <summary>
        /// Creates a new Token with the given parameters.
        /// </summary>
        /// <param name="type">The grammatical type of the scanned lexeme.</param>
        /// <param name="lexeme">The section of source that this token represents</param>
        /// <param name="literal">Literal value of this token.</param>
        /// <param name="line">Line number of the source this token was scanned from.</param>
        internal Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        /// <summary>
        /// Returns a string that represents the current Token.
        /// </summary>
        /// <returns>A string that represents the current Token.</returns>
        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
