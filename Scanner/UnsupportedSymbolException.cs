using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    class UnsupportedSymbolException : Exception
    {
        public UnsupportedSymbolException() : base() { }
        public UnsupportedSymbolException(string message) : base(message) { }
        public UnsupportedSymbolException(string message, Exception inner) : base(message, inner) { }
    }
}
