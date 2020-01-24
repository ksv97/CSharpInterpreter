using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class IncorrectSyntaxException : Exception
    {
        public IncorrectSyntaxException() : base() { }
        public IncorrectSyntaxException(string message) : base(message) { }
        public IncorrectSyntaxException(string message, Exception inner) : base(message, inner) { }
    }
}
