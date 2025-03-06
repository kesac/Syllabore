using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    public static class ListExtensions
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

    }
}
