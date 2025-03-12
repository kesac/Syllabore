using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    public static class TestExtensions
    {

        public static bool UnorderedListEquals(this List<FilterConstraint> first, List<FilterConstraint> second)
        {
            //return first.ToHashSet<Condition>().SetEquals(second.ToHashSet<Condition>());
            return first.Select(x => (x.Type, x.Value)).ToHashSet().SetEquals(second.Select(x => (x.Type, x.Value)).ToHashSet());
        }

        public static bool UnorderedListEquals(this List<Symbol> first, List<Symbol> second)
        {
            return first.Select(x => x.Value).ToHashSet<string>().SetEquals(second.Select(x => x.Value).ToHashSet<string>());
        }

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
