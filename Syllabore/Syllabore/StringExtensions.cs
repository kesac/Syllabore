using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// Convenience methods for dealing with strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits the specified string into an array of strings. Each
        /// string in the array represents one character of the original string.
        /// </summary>
        public static string[] Atomize(this string s)
        {
            return s.Select(x => x.ToString()).ToArray();
        }

    }
}
