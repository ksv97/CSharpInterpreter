using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public class Variable
    { 
        public string name;
        public bool isConst;

        public Variable(string name, bool isConst)
        {
            this.name = name;
            this.isConst = isConst;
        }
    }
}
