using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    /// <summary>
    /// Class that represents token received from scanner
    /// </summary>
    public class Token
    {
        public readonly TokenType TokenType;

        public readonly string Value;

        public Token(TokenType type, string value)
        {
            this.TokenType = type;
            this.Value = value;
        }

        public Token(string value, TokenType type)
        {
            this.TokenType = type;
            this.Value = value;
        }
    }
}
