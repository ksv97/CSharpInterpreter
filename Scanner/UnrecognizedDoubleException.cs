using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    class UnrecognizedDoubleException : Exception
    {
        public UnrecognizedDoubleException() : base() { }
        public UnrecognizedDoubleException(string message) : base(message) { }
        public UnrecognizedDoubleException(string message, Exception inner) : base(message, inner) { }
    }
}
