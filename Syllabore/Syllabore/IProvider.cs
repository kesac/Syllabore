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
        List<string> LeadingConsonants { get; set; }
        List<string> LeadingConsonantSequences { get; set; }
        List<string> Vowels { get; set; }
        List<string> VowelSequences { get; set; }
        List<string> TrailingConsonants { get; set; }
        List<string> TrailingConsonantSequences { get; set; }

        bool UseStartingSyllableLeadingVowels { get; set; }
        bool UseLeadingConsonants { get; set; }
        bool UseLeadingConsonantSequences { get; set; }
        bool UseVowelSequences { get; set; }
        bool UseTrailingConsonants { get; set; }
        bool UseTrailingConsonantSequences { get; set; }

        SyllableProviderProbability Probability { get; set; }

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
