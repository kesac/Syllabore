using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // Not sure if this should exist like this
    public class SyllableProviderProbability
    {

        /// <summary>
        /// In a starting syllable, this is the chance that a name starts with a vowel. This
        /// probability has no effect on syllables that are not meant to start a name.
        /// </summary>
        public double StartingSyllableLeadingVowel { get; private set; }

        /// <summary>
        /// If a starting syllable begins with a vowel, this is the chance it is a vowel sequence.
        /// </summary>
        public double StartingSyllableLeadingVowelSequence { get; private set; }

        /// <summary>
        /// Chance that the vowel in a syllable will be a vowel sequence.
        /// </summary>
        public double VowelSequence { get; private set; }

        /// <summary>
        /// If a syllable starts with a consonant, this is the change it becomes a consonant sequence.
        /// </summary>
        public double LeadingConsonantSequence { get; private set; }

        /// <summary>
        /// Chance that a syllable ends with a consonant instead of the syllable's vowel.
        /// </summary>
        public double TrailingConsonant { get; private set; }

        /// <summary>
        /// If a syllable ends with a consonant, this is the chance it becomes a consonant sequence.
        /// </summary>
        public double TrailingConsonantSequence { get; private set; }

        private SyllableProvider _parent;

        public SyllableProviderProbability(SyllableProvider parent)
        {
            _parent = parent;
        }

        public SyllableProviderProbability OfStartingSyllableLeadingVowels(double d)
        {

            if(d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.StartingSyllableLeadingVowel = d;
            return this;
        }

        public SyllableProviderProbability OfStartingSyllableLeadingVowelSequence(double d)
        {
            if (d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.StartingSyllableLeadingVowelSequence = d;
            return this;
        }

        public SyllableProviderProbability OfLeadingConsonantSequences(double d)
        {
            if (d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.LeadingConsonantSequence = d;
            return this;
        }

        public SyllableProviderProbability OfVowelSequences(double d)
        {
            if (d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.VowelSequence = d;
            return this;
        }

        public SyllableProviderProbability OfTrailingConsonants(double d)
        {
            if (d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.TrailingConsonant = d;
            return this;
        }

        public SyllableProviderProbability OfTrailingConsonantSequence(double d)
        {
            if (d < 0 || d > 1) { throw new InvalidOperationException("The probablity must be a value between 0 and 1 inclusive."); }

            this.TrailingConsonantSequence = d;
            return this;
        }

    }
}
