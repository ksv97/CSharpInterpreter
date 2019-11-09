using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanner
{
    /// <summary>
    /// Different dictionaries class that helps with language keywords and identifiers
    /// </summary>
    public static class Dictionaries
    {
        /// <summary>
        /// The list of keywords that's supported in language
        /// </summary>
        public static readonly List<string> LanguageKeywords = new List<string>()
        {
            "sqr",
            "sqrt",
            "print",
            "int",
            "double",
            "string",
            "boolean",
            "if",
            "else",
            "do",
            "while"
        };
    }
}
