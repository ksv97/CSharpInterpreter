using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    /// <summary>
    /// Enum with token types
    /// </summary>
    public enum TokenType
    {
        // language keywords part
        VARIABLE,
        SQR,
        SQRT,
        PRINT,
        INT,
        DOUBLE,
        STRING,
        BOOLEAN,
        IF,
        ELSE,
        DO,
        WHILE,

        // consts part
        TRUE,
        FALSE,
        INT_CONST,
        DOUBLE_CONST,
        STRING_CONST,

        // math operations
        PLUS,
        MINUS,
        DIV,
        MOD,
        MULTIPLY,
        SEMICOLON,

        // logical operators
        ASSIGN,
        EQUALS,
        NON_EQUALS,
        LOGICAL_OR,
        LOGICAL_AND,
        NOT,
        LESS,
        MORE,
        LESS_OR_EQUAL,
        MORE_OR_EQUAL,

        // special symbols
        CODEBLOCK_START,
        CODEBLOCK_END,
        PARANTHESIS_START,
        PARANTHESIS_END
    }
}
