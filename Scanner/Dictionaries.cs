using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    /// <summary>
    /// Different dictionaries class that helps with language keywords and identifiers
    /// </summary>
    internal static class Dictionaries
    {
        /// <summary>
        /// The list of keywords that's supported in language
        /// </summary>
        internal static readonly List<Token> LanguageKeywords = new List<Token>()
        {
            new Token("sqr", TokenType.SQR),
            new Token("sqrt", TokenType.SQRT),
            new Token("print", TokenType.PRINT),
            new Token("int", TokenType.INT),
            new Token("double", TokenType.DOUBLE),
            new Token("string", TokenType.STRING),
            new Token("boolean", TokenType.BOOLEAN),
            new Token("if", TokenType.IF),
            new Token("else", TokenType.ELSE),
            new Token("do", TokenType.DO),
            new Token("while", TokenType.WHILE),
            new Token("true", TokenType.TRUE),
            new Token("false", TokenType.FALSE)
        };
    }
}
