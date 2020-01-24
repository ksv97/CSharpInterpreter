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
            "console.log",
            "var",
            "const",
            "if",
            "else",
            "for",
            "function"
        };

        public static readonly List<string> UnresolvedSymbols = new List<string>()
        {
            "~",
            "`",
            "@",
            "#",
            "№",
            "$",
            "^",
            ":",
            "?",
            ""
        };
    }
}
