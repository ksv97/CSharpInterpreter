using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHarpInterpreter
{
    class LexemPriority
    {
        public int Priority;
        public string Name;
        public void Set(int priority, string name)
        {
            this.Priority = priority;
            this.Name = name;
        }
    }
}
