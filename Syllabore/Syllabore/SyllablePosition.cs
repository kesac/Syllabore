using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// The position of a syllable within a name.
    /// </summary>
    public enum SyllablePosition
    {
        Unknown = 0,

        /// <summary>
        /// The first syllable of a name.
        /// For names with only one syllable, this is the only syllable.
        /// </summary>
        Starting = 1,

        /// <summary>
        /// Denotes the syllables between the first and last syllables of a name.
        /// This position only exists for names that have three syllables or more.
        /// </summary>
        Inner = 2,

        /// <summary>
        /// The last syllable of a name.
        /// This position only exists for names with two syllables or more.
        /// </summary>
        Ending = 3,

        /// <summary>
        /// Represents any syllable position within a name.
        /// </summary>
        Any = 4
    }
}
