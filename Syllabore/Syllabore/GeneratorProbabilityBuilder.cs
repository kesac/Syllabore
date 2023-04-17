using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public class GeneratorProbabilityBuilder
    {
        private readonly GeneratorProbability Probability;

        public GeneratorProbabilityBuilder()
        {
            this.Probability = new GeneratorProbability();
        }

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

        // StartingSyllableLeadingVowelExists
        public GeneratorProbabilityBuilder OfLeadingVowelsInStartingSyllable(double probability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelExists = probability;
            return this;
        }

        // StartingSyllableLeadingVowelIsSequence
        public GeneratorProbabilityBuilder OfLeadingVowelIsSequenceInStartingSyllable(double probability)
        {
            this.Probability.ChanceStartingSyllableLeadingVowelIsSequence = probability;
            return this;
        }

        // LeadingConsonantExists
        public GeneratorProbabilityBuilder OfLeadingConsonants(double probability)
        {
            this.Probability.ChanceLeadingConsonantExists = probability;
            return this;
        }

        // LeadingConsonantIsSequence
        public GeneratorProbabilityBuilder OfLeadingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceLeadingConsonantIsSequence = probability;
            return this;
        }

        // VowelExists
        public GeneratorProbabilityBuilder OfVowels(double probability)
        {
            this.Probability.ChanceVowelExists = probability;
            return this;
        }

        // VowelIsSequence
        public GeneratorProbabilityBuilder OfVowelIsSequence(double probability)
        {
            this.Probability.ChanceVowelIsSequence = probability;
            return this;
        }

        // TrailingConsonantExists
        public GeneratorProbabilityBuilder OfTrailingConsonants(double probability)
        {
            this.Probability.ChanceTrailingConsonantExists = probability;
            return this;
        }

        // TrailingConsonantIsSequence
        public GeneratorProbabilityBuilder OfTrailingConsonantIsSequence(double probability)
        {
            this.Probability.ChanceTrailingConsonantIsSequence = probability;
            return this;
        }

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
