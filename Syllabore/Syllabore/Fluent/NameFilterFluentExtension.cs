using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Fluent
{
    public static class NameFilterFluentExtension
    {
        /// <summary>
        /// Makes a name invalid if it contains any of the specified substrings.
        /// </summary>
        public static NameFilter DoNotAllowSubstring(this NameFilter filter, params string[] substring)
        {
            foreach (string s in substring)
            {
                filter.Add(new FilterConstraint(FilterCondition.Contains, s));
            }

            return filter;
        }

        /// <summary>
        /// Makes a name invalid if it matches any of the specified regular expressions.
        /// </summary>
        public static NameFilter DoNotAllowRegex(this NameFilter filter, params string[] regex)
        {
            foreach (string r in regex)
            {
                filter.Add(new FilterConstraint(FilterCondition.MatchesPattern, r));
            }

            return filter;
        }

        /// <summary>
        /// Makes a name invalid if it starts with any of the specified substrings.
        /// </summary>
        public static NameFilter DoNotAllowStart(this NameFilter filter, params string[] prefixes)
        {
            foreach (string s in prefixes)
            {
                filter.Add(new FilterConstraint(FilterCondition.StartsWith, s));
            }

            return filter;
        }

        /// <summary>
        /// Makes a name invalid if it ends with any of the specified substrings.
        /// </summary>
        public static NameFilter DoNotAllowEnding(this NameFilter filter, params string[] suffixes)
        {
            foreach (string s in suffixes)
            {
                filter.Add(new FilterConstraint(FilterCondition.EndsWith, s));
            }

            return filter;
        }
    }
}
