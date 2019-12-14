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
            new Token("sqr", TokenType.SQR, -1, -1),
            new Token("sqrt", TokenType.SQRT, -1, -1),
            new Token("print", TokenType.PRINT, -1, -1),
            new Token("int", TokenType.INT, -1, -1),
            new Token("double", TokenType.DOUBLE, -1, -1),
            new Token("string", TokenType.STRING, -1, -1),
            new Token("boolean", TokenType.BOOLEAN, -1, -1),
            new Token("if", TokenType.IF, -1, -1),
            new Token("else", TokenType.ELSE, -1, -1),
            new Token("do", TokenType.DO, -1, -1),
            new Token("while", TokenType.WHILE, -1, -1),
            new Token("true", TokenType.TRUE, -1, -1),
            new Token("false", TokenType.FALSE, -1, -1)
        };
    }
}
