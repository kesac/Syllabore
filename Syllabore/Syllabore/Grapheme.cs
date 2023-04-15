using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A <see cref="Grapheme"/> is the most basic indivisible unit
    /// of a writing system. In Syllabore, they 
    /// usually represent a single vowel, consonant, or sequence.
    /// </para>
    /// <para>
    /// <see cref="Grapheme"/>s are used by <see cref="SyllableGenerator"/>s
    /// to construct syllables.
    /// </para>
    /// </summary>
    public class Grapheme : IWeighted
    {
        public string Value { get; set; }
        public int Weight { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="Grapheme"/> with the specified value.
        /// </summary>
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
