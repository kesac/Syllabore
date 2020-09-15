using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    public static class StringExtensions
    {
        public static string[] Atomize(this string s)
        {
            return s.Select(x => x.ToString()).ToArray();
        }
    }
}
