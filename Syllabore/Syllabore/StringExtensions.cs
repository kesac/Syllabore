using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static List<string> Atomize(this string s)
        {
            var stringInfo = new StringInfo(s);
            var trueLength = stringInfo.LengthInTextElements;

            var result = new List<string>();
            for(int i = 0; i < trueLength; i++)
            {
                result.Add(stringInfo.SubstringByTextElements(i, 1));
            }

            return result;
        }

    }
}
