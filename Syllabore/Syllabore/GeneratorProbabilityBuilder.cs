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
                ChanceFinalConsonantExists= existing.ChanceFinalConsonantExists,
                ChanceFinalConsonantIsSequence = existing.ChanceFinalConsonantIsSequence,
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
        /// Sets the probability that a leading vowel exists in the starting syllable
        /// and the probability that the vowel is a sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingVowelsInStartingSyllable(double probability, double sequenceProbability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelExists = probability;
            this.Probability.ChanceStartingSyllableLeadingVowelIsSequence = sequenceProbability;
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
        /// Sets the probability that a leading consonant exists within any syllable
        /// and the probability the consonant is a sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfLeadingConsonants(double probability, double sequenceProbability)
        {
            this.Probability.ChanceLeadingConsonantExists = probability;
            this.Probability.ChanceLeadingConsonantIsSequence = sequenceProbability;
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
        /// Sets the probability that a vowel exists within any syllable
        /// and the probability that the vowel is a sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfVowels(double probability, double sequenceProbability)
        {
            this.Probability.ChanceVowelExists = probability;
            this.Probability.ChanceVowelIsSequence = sequenceProbability;
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
        /// Sets the probability that a trailing consonant exists within any syllable
        /// and the probability that the consonant is a sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfTrailingConsonants(double probability, double sequenceProbability)
        {
            this.Probability.ChanceTrailingConsonantExists = probability;
            this.Probability.ChanceTrailingConsonantIsSequence = sequenceProbability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a final consonant exists within an ending syllable.
        /// </summary>
        public GeneratorProbabilityBuilder OfFinalConsonants(double probability)
        {
            this.Probability.ChanceFinalConsonantExists = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a final consonant exists within an ending syllable
        /// and the probability that the final consonant is a sequence.
        /// </summary>
        public GeneratorProbabilityBuilder OfFinalConsonants(double probability, double sequenceProbability)
        {
            this.Probability.ChanceFinalConsonantExists = probability;
            this.Probability.ChanceFinalConsonantIsSequence = sequenceProbability;
            return this;
        }

        #region Deprecated

        /// <summary>
        /// Sets the probability that a leading vowel is a sequence in the starting syllable.
        /// </summary>
        [Obsolete("Use OfLeadingVowelsInStartingSyllable() instead.")]
        public GeneratorProbabilityBuilder OfLeadingVowelIsSequenceInStartingSyllable(double probability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a leading consonant in any syllable is a consonant sequence.
        /// </summary>
        [Obsolete("Use OfLeadingConsonants() instead.")]
        public GeneratorProbabilityBuilder OfLeadingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceLeadingConsonantIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a vowel in any syllable is a vowel sequence.
        /// </summary>
        [Obsolete("Use OfVowels() instead.")]
        public GeneratorProbabilityBuilder OfVowelIsSequence(double probability)
        {
            this.Probability.ChanceVowelIsSequence = probability;
            return this;
        }


        /// <summary>
        /// Sets the probability that a trailing consonant in any syllable is a consonant sequence.
        /// </summary>
        [Obsolete("Use OfTrailingConsonants() instead.")]
        public GeneratorProbabilityBuilder OfTrailingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceTrailingConsonantIsSequence = probability;
            return this;
        }

        /// <summary>
        /// Sets the probability that a trailing consonant in any syllable is a consonant sequence.
        /// </summary>
        [Obsolete("Use OfFinalConsonants() instead.")]
        public GeneratorProbabilityBuilder OfFinalConsonantIsSequence(double probability)
        {
            this.Probability.ChanceFinalConsonantIsSequence = probability;
            return this;
        }

        #endregion

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
                ChanceFinalConsonantExists = this.Probability.ChanceFinalConsonantExists,
                ChanceFinalConsonantIsSequence = this.Probability.ChanceFinalConsonantIsSequence,
                ChanceVowelExists = this.Probability.ChanceVowelExists,
                ChanceVowelIsSequence = this.Probability.ChanceVowelIsSequence
            };
        }

    }
}
