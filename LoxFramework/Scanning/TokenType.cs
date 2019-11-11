namespace LoxFramework.Scanning
{
    /// <summary>
    /// The grammatical type of scanned <see cref="Token"/>s.
    /// </summary>
    public enum TokenType
    {
        // Single-character tokens
        /// <summary>
        /// ( token
        /// </summary>
        LEFT_PAREN,
        /// <summary>
        /// ) token
        /// </summary>
        RIGHT_PAREN,
        /// <summary>
        /// { token
        /// </summary>
        LEFT_BRACE,
        /// <summary>
        /// } token
        /// </summary>
        RIGHT_BRACE,
        /// <summary>
        /// , token
        /// </summary>
        COMMA,
        /// <summary>
        /// . token
        /// </summary>
        DOT,
        /// <summary>
        /// - token
        /// </summary>
        MINUS,
        /// <summary>
        /// + token
        /// </summary>
        PLUS,
        /// <summary>
        /// ; token
        /// </summary>
        SEMICOLON,
        /// <summary>
        /// / token
        /// </summary>
        SLASH,
        /// <summary>
        /// * token
        /// </summary>
        STAR,

        // One or two character tokens
        /// <summary>
        /// ! token
        /// </summary>
        BANG,
        /// <summary>
        /// != token
        /// </summary>
        BANG_EQUAL,
        /// <summary>
        /// = token
        /// </summary>
        EQUAL,
        /// <summary>
        /// == token
        /// </summary>
        EQUAL_EQUAL,
        /// <summary>
        /// > token
        /// </summary>
        GREATER,
        /// <summary>
        /// >= token
        /// </summary>
        GREATER_EQUAL,
        /// <summary>
        /// &lt; token
        /// </summary>
        LESS,
        /// <summary>
        /// &lt;= token
        /// </summary>
        LESS_EQUAL,

        // Literals
        /// <summary>
        /// identifier token
        /// </summary>
        IDENTIFIER,
        /// <summary>
        /// string token
        /// </summary>
        STRING,
        /// <summary>
        /// number token
        /// </summary>
        NUMBER,

        // Keywords
        /// <summary>
        /// &amp; token
        /// </summary>
        AND,
        /// <summary>
        /// class token
        /// </summary>
        CLASS,
        /// <summary>
        /// else token
        /// </summary>
        ELSE,
        /// <summary>
        /// false token
        /// </summary>
        FALSE,
        /// <summary>
        /// function token
        /// </summary>
        FUN,
        /// <summary>
        /// for token
        /// </summary>
        FOR,
        /// <summary>
        /// if token
        /// </summary>
        IF,
        /// <summary>
        /// nil token
        /// </summary>
        NIL,
        /// <summary>
        /// or token
        /// </summary>
        OR,
        /// <summary>
        /// print token
        /// </summary>
        PRINT,
        /// <summary>
        /// return token
        /// </summary>
        RETURN,
        /// <summary>
        /// super token
        /// </summary>
        SUPER,
        /// <summary>
        /// this token
        /// </summary>
        THIS,
        /// <summary>
        /// true token
        /// </summary>
        TRUE,
        /// <summary>
        /// var token
        /// </summary>
        VAR,
        /// <summary>
        /// while token
        /// </summary>
        WHILE,

        /// <summary>
        /// End of file token
        /// </summary>
        EOF
    }
}
