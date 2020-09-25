using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    public static class StringExtensions
    {
        public static string[] Atomize(this string s)
        {
            return s.Select(x => x.ToString()).ToArray();
        }

        public static bool StartsWith(this string s, params string[] prefixes)
        {
            foreach(var prefix in prefixes)
            {
                if (s.StartsWith(prefix))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool StartsWithVowel(this string s)
        {
            return Regex.IsMatch(s, "^[aieouAEIOU]$");
        }
        public static bool StartsWithVowel(this Name n)
        {
            return Regex.IsMatch(n.ToString(), "^[aieouAEIOU]");
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

        public static bool EndsWithVowel(this string s)
        {
            return Regex.IsMatch(s, "[aieouAEIOU]$");
        }

        public static bool EndsWithVowel(this Name n)
        {
            return Regex.IsMatch(n.ToString(), "[aieouAEIOU]$");
        }

        public static bool EndsWithConsonant(this string s)
        {
            return !s.EndsWithVowel();
        }

        public static bool EndsWithConsonant(this Name n)
        {
            return !n.EndsWithVowel();
        }

    }
}
