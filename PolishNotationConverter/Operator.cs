using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyzer
{
    public class Operator
    {
        private static List<Operator> operators;

        public string Value { get; }

        public int Priority { get; }

        public Operator(string value, int priority)
        {
            this.Value = value;
            this.Priority = priority;
        }

        public static List<Operator> GetAvailableOperators()
        {
            if (operators != null)
                return operators;

            operators = new List<Operator>()
            {
                new Operator("{", 0),
                new Operator("(", 0),
                new Operator("if", 0),
            };

            return operators;
        }
    }
}
