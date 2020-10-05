using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    public static class ListExtensions
    {
        public static bool UnorderedListEquals<T>(this List<T> list, List<T> otherList)
        {
            return list.ToHashSet<T>().SetEquals(otherList.ToHashSet<T>());
        }

    }
}
