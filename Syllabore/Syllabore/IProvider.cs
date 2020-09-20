using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables useful for building names.
    /// </summary>
    public interface IProvider
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
