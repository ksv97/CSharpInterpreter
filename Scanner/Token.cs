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

        public readonly int Line;

        public Token(TokenType type, string value, int posiiton, int line)
        {
            this.TokenType = type;
            this.Value = value;
            this.Position = posiiton + 1;
            this.Line = line + 1;   
        }

        public Token(string value, TokenType type, int position, int line)
        {
            this.TokenType = type;
            this.Value = value;
            this.Position = position + 1;
            this.Line = line + 1;
        }
    }
}
