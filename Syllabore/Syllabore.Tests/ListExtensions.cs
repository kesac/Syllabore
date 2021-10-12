using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    public static class ListExtensions
    {
        public static bool UnorderedListEquals(this List<string> first, List<string> second)
        {
            return first.ToHashSet<string>().SetEquals(second.ToHashSet<string>());
        }

        public static bool UnorderedListEquals(this List<Grapheme> first, List<Grapheme> second)
        {
            return first.Select(x => x.Value).ToHashSet<string>().SetEquals(second.Select(x => x.Value).ToHashSet<string>());
        }

    }
}
