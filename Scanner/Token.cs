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
        public int Id;

        public TokenType TokenType;

        public string Value;

        public string Type;

        public Token(TokenType type, string value)
        {
            this.TokenType = type;
            this.Value = value;
        }
        public Token()
        {

        }
    }
}
