using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// A <see cref="Grapheme"/> is an indivisible unit of a 
    /// writing system. In Syllabore, <see cref="Grapheme">Graphemes</see> 
    /// are used to represent vowels, consonants, or sequences.
    /// <see cref="Grapheme">Graphemes</see> are used directly by 
    /// <see cref="SyllableGenerator">SyllableGenerators</see>
    /// when constructing syllables.
    /// </para>
    /// </summary>
    public class Grapheme : IWeighted
    {
        /// <summary>
        /// The vowel, consonant, or sequence that 
        /// this <see cref="Grapheme"/> represents.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The weight of occurrence for this
        /// <see cref="Grapheme"/>. By default the
        /// value is 1.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="Grapheme"/> with the specified value.
        /// </summary>
        public Grapheme(string value)
        {
            this.Value = value;
            this.Weight = 1;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Grapheme"/>.
        /// </summary>
        public override string ToString()
        {
            return this.Value;
        }

    }
}
