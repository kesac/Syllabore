using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables that can be sequenced into names.
    /// </summary>
    public interface ISyllableGenerator
    {
        /// <summary>
        /// Generates a random syllable suitable for starting name.
        /// </summary>
        string NextStartingSyllable();

        /// <summary>
        /// Generates a random syllable suitable for any part of a name.
        /// </summary>
        string NextSyllable();

        /// <summary>
        /// Generates a random syllable suitable for ending a name.
        /// </summary>
        string NextEndingSyllable();

    }
}
