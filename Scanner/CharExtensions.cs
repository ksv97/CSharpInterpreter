using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    public static class CharExtensions
    {
        public static bool IsWhiteSpace (this char value)
        {
            return value == ' ';
        }

        /// <summary>
        /// Determintes whether a specified char value is digit
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDigit (this char value)
        {
            char[] digits = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            return digits.Contains(value);
        }

        /// <summary>
        /// Determines whether the char value is double const delimeter
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDoubleConstDelimeter (this char value)
        {
            return value == '.';
        }
    }
}
