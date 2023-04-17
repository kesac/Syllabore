using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// An intermediate class used to build a <see cref="GeneratorProbability"/>.
    /// </summary>
    public class GeneratorProbabilityBuilder
    {
        private readonly GeneratorProbability Probability;

        /// <summary>
        /// Creates a new instance of <see cref="GeneratorProbabilityBuilder"/>.
        /// </summary>
        public GeneratorProbabilityBuilder()
        {
            this.Probability = new GeneratorProbability();
        }

        /// <summary>
        /// Creates a new instance of <see cref="GeneratorProbabilityBuilder"/> with the
        /// specified <see cref="GeneratorProbability"/> as a starting point.
        /// </summary>
        public GeneratorProbabilityBuilder(GeneratorProbability existing)
        {
            this.Probability = new GeneratorProbability()
            {
                ChanceLeadingConsonantExists = existing.ChanceLeadingConsonantExists,
                ChanceLeadingConsonantIsSequence = existing.ChanceLeadingConsonantIsSequence,
                ChanceStartingSyllableLeadingVowelExists = existing.ChanceStartingSyllableLeadingVowelExists,
                ChanceStartingSyllableLeadingVowelIsSequence = existing.ChanceStartingSyllableLeadingVowelIsSequence,
                ChanceTrailingConsonantExists = existing.ChanceTrailingConsonantExists,
                ChanceTrailingConsonantIsSequence = existing.ChanceTrailingConsonantIsSequence,
                ChanceVowelExists = existing.ChanceVowelExists,
                ChanceVowelIsSequence = existing.ChanceVowelIsSequence
            };
        }

        /// <summary>
        /// Sets the probability that a leading vowel exists in the starting syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingVowelsInStartingSyllable(double probability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelExists = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a leading vowel is a sequence in the starting syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingVowelIsSequenceInStartingSyllable(double probability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a leading consonant exists within any syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingConsonants(double probability)
        {
            this.Probability.ChanceLeadingConsonantExists = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a leading consonant in any syllable is a consonant sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceLeadingConsonantIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a vowel exists within any syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfVowels(double probability)
        {
            this.Probability.ChanceVowelExists = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a vowel in any syllable is a vowel sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfVowelIsSequence(double probability)
        {
            this.Probability.ChanceVowelIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a trailing consonant exists within any syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfTrailingConsonants(double probability)
        {
            this.Probability.ChanceTrailingConsonantExists = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a trailing consonant in any syllable is a consonant sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfTrailingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceTrailingConsonantIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Returns a new instance of <see cref="GeneratorProbability"/> with the values built
        /// through this <see cref="GeneratorProbabilityBuilder"/>.
        /// </summary>
        public GeneratorProbability ToProbability()
        {
            // We return a new instance so that it is safe to keep using the builder
            // after a call to ToProbability().
            return new GeneratorProbability()
            {
                ChanceLeadingConsonantExists = this.Probability.ChanceLeadingConsonantExists,
                ChanceLeadingConsonantIsSequence = this.Probability.ChanceLeadingConsonantIsSequence,
                ChanceStartingSyllableLeadingVowelExists = this.Probability.ChanceStartingSyllableLeadingVowelExists,
                ChanceStartingSyllableLeadingVowelIsSequence = this.Probability.ChanceStartingSyllableLeadingVowelIsSequence,
                ChanceTrailingConsonantExists = this.Probability.ChanceTrailingConsonantExists,
                ChanceTrailingConsonantIsSequence = this.Probability.ChanceTrailingConsonantIsSequence,
                ChanceVowelExists = this.Probability.ChanceVowelExists,
                ChanceVowelIsSequence = this.Probability.ChanceVowelIsSequence
            };
        }

    }
}
