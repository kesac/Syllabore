using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    public class SyllableProviderProbability
    {
        public class SyllableProbability
        {
            [JsonIgnore]
            public SyllableProviderProbability Parent { get; set; }

            public double? ChanceLeadingVowelExists { get; set; }         // This is specific to starting syllables (ie. calls to NextStartingSyllable())
            public double? ChanceLeadingVowelBecomesSequence { get; set; } // This is specific to starting syllables (ie. calls to NextStartingSyllable())

            public SyllableProviderProbability LeadingVowelExists(double probability)
            {
                this.ChanceLeadingVowelExists = probability;
                return Parent;
            }

            public SyllableProviderProbability LeadingVowelBecomesSequence(double probability)
            {
                this.ChanceLeadingVowelBecomesSequence = probability;
                return Parent;
            }

        }

        public SyllableProbability StartingSyllable { get; set; } = new SyllableProbability();

        public double? ChanceLeadingConsonantExists { get; set; }
        public double? ChanceLeadingConsonantBecomesSequence { get; set; }
        public double? ChanceVowelExists { get; set; }
        public double? ChanceVowelBecomesSequence { get; set; }
        public double? ChanceTrailingConsonantExists { get; set; }
        public double? ChanceTrailingConsonantBecomesSequence { get; set; }

        public SyllableProviderProbability()
        {
            this.StartingSyllable = new SyllableProbability() { Parent = this };
        }

        public SyllableProviderProbability LeadingConsonantExists(double probability)
        {
            this.ChanceLeadingConsonantExists = probability;
            return this;
        }

        public SyllableProviderProbability LeadingConsonantBecomesSequence(double probability)
        {
            this.ChanceLeadingConsonantBecomesSequence = probability;
            return this;
        }

        public SyllableProviderProbability VowelExists(double probability)
        {
            this.ChanceVowelExists = probability;
            return this;
        }

        public SyllableProviderProbability VowelBecomesSequence(double probability)
        {
            this.ChanceVowelBecomesSequence = probability;
            return this;
        }

        public SyllableProviderProbability TrailingConsonantExists(double probability)
        {
            this.ChanceTrailingConsonantExists = probability;
            return this;
        }

        public SyllableProviderProbability TrailingConsonantBecomesSequence(double probability)
        {
            this.ChanceTrailingConsonantBecomesSequence = probability;
            return this;
        }

    }
}
