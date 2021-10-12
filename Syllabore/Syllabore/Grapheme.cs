using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // Represents a weighted vowel or consonant.
    public class Grapheme
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
