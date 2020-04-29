using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Represents a generated name. Can be returned by NameGenerator for callers
    /// to access the syllables of a name.
    /// </summary>
    public struct Name
    {
        public string[] Syllables { get; set; }
        public Name(string[] syllables)
        {
            this.Syllables = new string[syllables.Length];
            syllables.CopyTo(this.Syllables, 0);
        }

        public override string ToString()
        {
            var result = string.Join(string.Empty, this.Syllables);
            return result.Substring(0, 1).ToUpper() + result.Substring(1).ToLower();
        }
    }
}
