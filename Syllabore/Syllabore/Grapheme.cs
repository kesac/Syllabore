using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Represents a vowel or consonant used
    /// to construct a syllable.
    /// </summary>
    public class Grapheme : IWeighted
    {
        public string Value { get; set; }
        public int Weight { get; set; }

        public Grapheme(string value)
        {
            this.Value = value;
            this.Weight = 1;
        }

        public override string ToString()
        {
            return this.Value;
        }

    }
}
