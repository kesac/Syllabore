using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables based on a set of configurable vowels and consonants.
    /// </summary>
    [Serializable]
    public class SyllableProvider : IProvider
    {
        private Random Random { get; set; }
        public List<string> LeadingConsonants { get; set; }
        public List<string> LeadingConsonantSequences { get; set; }
        public List<string> Vowels { get; set; }
        public List<string> VowelSequences { get; set; }
        public List<string> TrailingConsonants { get; set; }
        public List<string> TrailingConsonantSequences { get; set; }

        public bool UseStartingSyllableLeadingVowels { get; set; }
        public bool UseLeadingConsonants { get; set; }
        public bool UseLeadingConsonantSequences { get; set; }
        public bool UseVowelSequences { get; set; }
        public bool UseTrailingConsonants { get; set; }
        public bool UseTrailingConsonantSequences { get; set; }

        public SyllableProviderProbability Probability { get; set; }

        public SyllableProvider()
        {
            this.Random = new Random();

            this.LeadingConsonants = new List<string>();
            this.LeadingConsonantSequences = new List<string>();
            this.Vowels = new List<string>();
            this.VowelSequences = new List<string>();
            this.TrailingConsonants = new List<string>();
            this.TrailingConsonantSequences = new List<string>();

            // These are automatically allowed once they are defined
            this.AllowStartingSyllableLeadingVowels();
            this.DisallowLeadingConsonants();
            this.DisallowLeadingConsonantSequences();
            this.DisallowVowelSequences();
            this.DisallowTrailingConsonants();
            this.DisallowTrailingConsonantSequences();

            this.Probability = new SyllableProviderProbability(this);
            this.Probability.OfStartingSyllableLeadingVowels(0.10);
            this.Probability.OfStartingSyllableLeadingVowelSequence(0.05);
            this.Probability.OfLeadingConsonantSequences(0.20);
            this.Probability.OfVowelSequences(0.20);
            this.Probability.OfTrailingConsonants(0.10);
            this.Probability.OfTrailingConsonantSequence(0.10);
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can occur before a vowel.
        /// </summary>
        public SyllableProvider WithLeadingConsonants(params string[] consonants)
        {
            foreach (var c in consonants)
            {
                this.LeadingConsonants.AddRange(c.Atomize());
            }

            this.AllowLeadingConsonants();

            return this;
        }

        /// <summary>
        /// Adds the specified consonant sequences as sequences that can occur before a vowel.
        /// </summary>
        public SyllableProvider WithLeadingConsonantSequences(params string[] consonantSequences)
        {
            this.LeadingConsonantSequences.AddRange(consonantSequences);

            this.AllowLeadingConsonantSequences();

            return this;
        }

        /// <summary>
        /// Adds the specified vowels as vowels that can be used to form the 'center' of syllables.
        /// </summary>
        public SyllableProvider WithVowels(params string[] vowels)
        {
            foreach (var v in vowels)
            {
                this.Vowels.AddRange(v.Atomize());
            }

            return this;
        }

        /// <summary>
        /// Adds the specified vowel sequences as sequences that can be used to form the 'center' of syllables.
        /// </summary>
        public SyllableProvider WithVowelSequences(params string[] vowelSequences)
        {
            this.VowelSequences.AddRange(vowelSequences);

            this.AllowVowelSequences();

            return this;
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can appear after a vowel.
        /// </summary>
        public SyllableProvider WithTrailingConsonants(params string[] consonants)
        {
            foreach (var c in consonants)
            {
                this.TrailingConsonants.AddRange(c.Atomize());
            }

            this.AllowTrailingConsonants();

            return this;
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can appear after a vowel.
        /// </summary>
        public SyllableProvider WithTrailingConsonantSequences(params string[] consonantSequences)
        {
            this.TrailingConsonantSequences.AddRange(consonantSequences);

            this.AllowTrailingConsonantSequences();

            return this;
        }

        // TODO - Docs
        
        public SyllableProvider AllowStartingSyllableLeadingVowels() { this.UseStartingSyllableLeadingVowels = true; return this; }
        public SyllableProvider AllowLeadingConsonants() { this.UseLeadingConsonants = true; return this; }
        public SyllableProvider AllowLeadingConsonantSequences() { this.UseLeadingConsonantSequences = true; return this; }
        public SyllableProvider AllowVowelSequences() { this.UseVowelSequences = true; return this; }
        public SyllableProvider AllowTrailingConsonants() { this.UseTrailingConsonants = true; return this; }
        public SyllableProvider AllowTrailingConsonantSequences() { this.UseTrailingConsonantSequences = true; return this; }
        public SyllableProvider DisallowStartingSyllableLeadingVowels() { this.UseStartingSyllableLeadingVowels = false; return this; }
        public SyllableProvider DisallowLeadingConsonants() { this.UseLeadingConsonants = false; return this; }
        public SyllableProvider DisallowLeadingConsonantSequences() { this.UseLeadingConsonantSequences = false; return this; }
        public SyllableProvider DisallowVowelSequences() { this.UseVowelSequences = false; return this; }
        public SyllableProvider DisallowTrailingConsonants() { this.UseTrailingConsonants = false; return this; }
        public SyllableProvider DisallowTrailingConsonantSequences() { this.UseTrailingConsonantSequences = false; return this; }

        public SyllableProvider WithProbability(Func<SyllableProviderProbability, SyllableProviderProbability> configure)
        {
            this.Probability = configure(this.Probability);
            return this;
        }

        private string NextLeadingConsonant()
        {
            if(this.LeadingConsonants.Count > 0)
            {
                return this.LeadingConsonants[this.Random.Next(this.LeadingConsonants.Count)];
            }
            else
            {
                throw new InvalidOperationException("No leading consonants defined.");
            }
        }

        private string NextLeadingConsonantSequence()
        {
            if (this.LeadingConsonantSequences.Count > 0)
            {
                return this.LeadingConsonantSequences[this.Random.Next(this.LeadingConsonantSequences.Count)];
            }
            else
            {
                throw new InvalidOperationException("No leading consonant sequences defined.");
            }
        }

        private string NextVowel()
        {
            if (this.Vowels.Count > 0)
            {
                return this.Vowels[this.Random.Next(this.Vowels.Count)];
            }
            else
            {
                throw new InvalidOperationException("A vowel was required, but no vowels defined.");
            }
        }

        private string NextVowelSequence()
        {
            if (this.VowelSequences.Count > 0)
            {
                return this.VowelSequences[this.Random.Next(this.VowelSequences.Count)];
            }
            else
            {
                throw new InvalidOperationException("A vowel sequence was required, but no vowel sequences defined.");
            }
        }

        private string NextTrailingConsonant()
        {
            if (this.TrailingConsonants.Count > 0)
            {
                return this.TrailingConsonants[this.Random.Next(this.TrailingConsonants.Count)];
            }
            else
            {
                throw new InvalidOperationException("No trailing consonants defined.");
            }
        }

        private string NextTrailingConsonantSequence()
        {
            if (this.TrailingConsonantSequences.Count > 0)
            {
                return this.TrailingConsonantSequences[this.Random.Next(this.TrailingConsonantSequences.Count)];
            }
            else
            {
                throw new InvalidOperationException("No trailing consonant sequences defined.");
            }

        }


        public string NextStartingSyllable()
        {
            return GenerateSyllable(true, true);
        }

        public string NextSyllable()
        {
            return GenerateSyllable(false, false);
        }


        public string NextEndingSyllable()
        {
            return GenerateSyllable(false, true);
        }

        private string GenerateSyllable(bool isStartingSyllable, bool allowSequences)
        {
            var output = new StringBuilder();

            if (isStartingSyllable && this.UseStartingSyllableLeadingVowels && this.Random.NextDouble() < this.Probability.StartingSyllableLeadingVowel)
            {
                if (this.UseVowelSequences && this.Random.NextDouble() < this.Probability.StartingSyllableLeadingVowelSequenceProbability)
                {
                    output.Append(this.NextVowelSequence());
                }
                else
                {
                    output.Append(this.NextVowel());
                }
            }
            else
            {

                if (this.UseLeadingConsonantSequences && allowSequences && this.Random.NextDouble() < this.Probability.LeadingConsonantSequenceProbability)
                {
                    output.Append(this.NextLeadingConsonantSequence());
                }
                else if(this.UseLeadingConsonants)
                {
                    output.Append(this.NextLeadingConsonant());
                }

                if (this.UseVowelSequences && allowSequences && this.Random.NextDouble() < this.Probability.VowelSequence)
                {
                    output.Append(this.NextVowelSequence());
                }
                else
                {
                    output.Append(this.NextVowel());
                }
            }

            if (this.UseTrailingConsonants && this.Random.NextDouble() < this.Probability.TrailingConsonant)
            {
                output.Append(this.NextTrailingConsonant());
            }
            else if (this.UseTrailingConsonantSequences && allowSequences && this.Random.NextDouble() < this.Probability.TrailingConsonantSequence)
            {
                output.Append(this.NextTrailingConsonantSequence());
            }

            return output.ToString();
        }

    }
}
