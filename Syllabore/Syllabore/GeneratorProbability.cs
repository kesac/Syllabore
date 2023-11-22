using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// Contains the probability settings of a <see cref="SyllableGenerator"/>.
    /// </summary>
    public class GeneratorProbability
    {
        /// <summary>
        /// The probability that a leading vowel exists in the starting syllable. In other
        /// words, this is the probability that a name starts with a vowel rather than a consonant.
        /// </summary>
        public double? ChanceStartingSyllableLeadingVowelExists { get; set; }

        /// <summary>
        /// The probability that a leading vowel in the starting syllable is a sequence. In other
        /// words, this is the probability that a name starts with a vowel sequence rather than a consonant.
        /// </summary>
        public double? ChanceStartingSyllableLeadingVowelIsSequence { get; set; }

        /// <summary>
        /// The probability that a leading consonant exists in a syllable. A leading
        /// consonant is a consonant that appears before a vowel in a syllable.
        /// </summary>
        public double? ChanceLeadingConsonantExists { get; set; }

        /// <summary>
        /// The probability that a leading consonant in a syllable is a sequence. A leading
        /// consonant sequence is a consonant sequence that appears before a vowel in a syllable.
        /// </summary>
        public double? ChanceLeadingConsonantIsSequence { get; set; }

        /// <summary>
        /// The probability that a vowel exists in a syllable.
        /// </summary>
        public double? ChanceVowelExists { get; set; }

        /// <summary>
        /// The probability that a vowel in a syllable is a sequence.
        /// </summary>
        public double? ChanceVowelIsSequence { get; set; }

        /// <summary>
        /// The probability that a trailing consonant exists in a syllable. A trailing
        /// consonant is a consonant that appears after a vowel in a syllable.
        /// </summary>
        public double? ChanceTrailingConsonantExists { get; set; }

        /// <summary>
        /// The probability that a trailing consonant in a syllable is a sequence. A trailing
        /// consonant is a consonant that appears after a vowel in a syllable.
        /// </summary>
        public double? ChanceTrailingConsonantIsSequence { get; set; }

        /// <summary>
        /// The probability that a final consonant exists in a syllable. Final consonants
        /// are the same as trailing consonants excecpt they only appear in the final
        /// syllable of a name.
        /// </summary>
        public double? ChanceFinalConsonantExists { get; set; }

        /// <summary>
        /// The probability that a final consonant in a syllable is a sequence. Final consonants
        /// are the same as trailing consonants excecpt they only appear in the final
        /// syllable of a name.
        /// </summary>
        public double? ChanceFinalConsonantIsSequence { get; set; }
    }
}
