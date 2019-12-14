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

        public readonly int Position;

        public Token(TokenType type, string value, int posiiton)
        {
            this.TokenType = type;
            this.Value = value;
            this.Position = posiiton;
        }

        public Token(string value, TokenType type, int position)
        {
            this.TokenType = type;
            this.Value = value;
            this.Position = position;
        }
    }
}
