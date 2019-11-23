using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class ParseErrorException : Exception
    {
        public ParseErrorException(int position, string message)
        {
            this.Message = $"Parse error occured at position {position}. {message}";
        }

        public new string Message { get; private set; }

    }
}
