using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Syllabore.Tests
{
    public static class StringExtensions
    {

        public static bool StartsWith(this string s, params string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                if (s.StartsWith(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(this string s, params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (s.Contains(substring))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EndsWith(this string s, params string[] suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (s.EndsWith(suffix))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
