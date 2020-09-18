using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // Not sure if this should exist like this
    public class ConfigurableSyllableProviderProbability
    {

        /// <summary>
        /// In a starting syllable, this is the chance that a name starts with a vowel. This
        /// probability has no effect on syllables that are not meant to start an name.
        /// </summary>
        public double StartingSyllableLeadingVowel { get; private set; }

        /// <summary>
        /// If a starting syllable begins with a vowel, this is the chance it is a vowel sequence.
        /// </summary>
        public double StartingSyllableLeadingVowelSequenceProbability { get; private set; }

        /// <summary>
        /// Chance that the vowel in a syllable will be a vowel sequence.
        /// </summary>
        public double VowelSequence { get; private set; }

        /// <summary>
        /// Chance that a syllable starts with a consonant sequence instead of just a consonant.
        /// </summary>
        public double LeadingConsonantSequenceProbability { get; private set; }

        /// <summary>
        /// Chance that a syllable ends with a consonant instead of the syllable's vowel.
        /// </summary>
        public double TrailingConsonant { get; private set; }

        /// <summary>
        /// If a syllable ends with a consonant, this is the chance it is a consonant sequence.
        /// </summary>
        public double TrailingConsonantSequence { get; private set; }


        private ConfigurableSyllableProvider _parent;

        public ConfigurableSyllableProviderProbability(ConfigurableSyllableProvider parent)
        {
            _parent = parent;
        }

        public ConfigurableSyllableProvider Confirm()
        {
            return _parent;
        }

        // TODO - Docs and argument validation
        public ConfigurableSyllableProviderProbability OfStartingSyllableLeadingVowels(double d)
        {
            this.StartingSyllableLeadingVowel = d;
            return this;
        }

        public ConfigurableSyllableProviderProbability OfStartingSyllableLeadingVowelSequence(double d)
        {
            this.StartingSyllableLeadingVowelSequenceProbability = d;
            return this;
        }

        // TODO - Docs and argument validation
        public ConfigurableSyllableProviderProbability OfLeadingConsonantSequences(double d)
        {
            this.LeadingConsonantSequenceProbability = d;
            return this;
        }

        // TODO - Docs and argument validation
        public ConfigurableSyllableProviderProbability OfVowelSequences(double d)
        {
            this.VowelSequence = d;
            return this;
        }

        // TODO - Docs and argument validation
        public ConfigurableSyllableProviderProbability OfTrailingConsonants(double d)
        {
            this.TrailingConsonant = d;
            return this;
        }

        // TODO - Docs and argument validation
        public ConfigurableSyllableProviderProbability OfTrailingConsonantSequence(double d)
        {
            this.TrailingConsonantSequence = d;
            return this;
        }

    }
}
