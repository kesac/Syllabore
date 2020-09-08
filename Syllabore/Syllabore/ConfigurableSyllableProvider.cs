using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables based on a set of configurable vowels and consonants.
    /// </summary>
    public class ConfigurableSyllableProvider : ISyllableProvider
    {
        private Random Random { get; set; }
        private List<string> LeadingConsonants { get; set; }
        private List<string> LeadingConsonantSequences { get; set; }
        private List<string> Vowels { get; set; }
        private List<string> VowelSequences { get; set; }
        private List<string> TrailingConsonants { get; set; }
        private List<string> TrailingConsonantSequences { get; set; }


        public bool UseLeadingConsonants { get; set; }
        public bool UseLeadingConsonantSequences { get; set; }
        public bool UseVowelSequences { get; set; }
        public bool UseTrailingConsonants { get; set; }
        public bool UseTrailingConsonantSequences { get; set; }

        public double LeadingVowelProbability { get; set; }
        public double LeadingConsonantSequenceProbability { get; set; }
        public double VowelSequenceProbability { get; set; }
        public double TrailingConsonantProbability { get; set; }
        public double TrailingConsonantSequenceProbability { get; set; }

        public ConfigurableSyllableProvider()
        {
            this.Random = new Random();

            this.LeadingConsonants = new List<string>();
            this.LeadingConsonantSequences = new List<string>();
            this.Vowels = new List<string>();
            this.VowelSequences = new List<string>();
            this.TrailingConsonants = new List<string>();
            this.TrailingConsonantSequences = new List<string>();

            this.UseLeadingConsonants = true;
            this.UseLeadingConsonantSequences = true;
            this.UseVowelSequences = true;
            this.UseTrailingConsonants = true;
            this.UseTrailingConsonantSequences = true;

            this.LeadingVowelProbability = 0.10;
            this.LeadingConsonantSequenceProbability = 0.20;
            this.VowelSequenceProbability = 0.20;
            this.TrailingConsonantProbability = 0.10;
            this.TrailingConsonantSequenceProbability = 0.10;
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can occur before a vowel.
        /// </summary>
        public void AddLeadingConsonant(params string[] consonants)
        {
            this.LeadingConsonants.AddRange(consonants);
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can occur before a vowel.
        /// </summary>
        public void AddLeadingConsonantSequence(params string[] consonantSequences)
        {
            this.LeadingConsonantSequences.AddRange(consonantSequences);
        }

        /// <summary>
        /// Adds the specified vowels as vowels that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowel(params string[] vowels)
        {
            this.Vowels.AddRange(vowels);
        }

        /// <summary>
        /// Adds the specified vowel sequences as sequences that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowelSequence(params string[] vowelSequences)
        {
            this.VowelSequences.AddRange(vowelSequences);
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can appear after a vowel.
        /// </summary>
        public void AddTrailingConsonant(params string[] consonants)
        {
            this.TrailingConsonants.AddRange(consonants);
        }


        /// <summary>
        /// Adds the specified consonant sequences as sequences that can appear after a vowel.
        /// </summary>
        public void AddTrailingConsonantSequence(params string[] consonantSequences)
        {
            this.TrailingConsonantSequences.AddRange(consonantSequences);
        }

        /// <summary>
        /// Returns list of all possible vowels and vowel sequences this provider can generate.
        /// </summary>
        public List<string> GetAllVowels()
        {
            var result = new List<string>();
            result.AddRange(this.Vowels);
            result.AddRange(this.VowelSequences);
            return result;
        }

        /// <summary>
        /// Returns list of all possible leading consonants and consonant sequences this provider can generate.
        /// </summary>
        public List<string> GetAllLeadingConsonants()
        {
            var result = new List<string>();
            result.AddRange(this.LeadingConsonants);
            result.AddRange(this.LeadingConsonantSequences);
            return result;
        }

        /// <summary>
        /// Returns list of all possible trailing consonants and consonant sequences this provider can generate.
        /// </summary>
        public List<string> GetAllTrailingConsonants()
        {
            var result = new List<string>();
            result.AddRange(this.TrailingConsonants);
            result.AddRange(this.TrailingConsonantSequences);
            return result;
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
                throw new InvalidOperationException("No vowels defined.");
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
                throw new InvalidOperationException("No vowel sequences defined.");
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

        private string GenerateSyllable(bool allowLeadingVowel, bool allowSequences)
        {
            var output = new StringBuilder();

            if (allowLeadingVowel && this.Random.NextDouble() < this.LeadingVowelProbability)
            {
                output.Append(this.NextVowel());
            }
            else
            {

                if (this.UseLeadingConsonantSequences && allowSequences && this.Random.NextDouble() < this.LeadingConsonantSequenceProbability)
                {
                    output.Append(this.NextLeadingConsonantSequence());
                }
                else if(this.UseLeadingConsonants)
                {
                    output.Append(this.NextLeadingConsonant());
                }

                if (this.UseVowelSequences && allowSequences && this.Random.NextDouble() < this.VowelSequenceProbability)
                {
                    output.Append(this.NextVowelSequence());
                }
                else
                {
                    output.Append(this.NextVowel());
                }
            }

            if (this.UseTrailingConsonants && this.Random.NextDouble() < this.TrailingConsonantProbability)
            {
                output.Append(this.NextTrailingConsonant());
            }
            else if (this.UseTrailingConsonantSequences && allowSequences && this.Random.NextDouble() < this.TrailingConsonantSequenceProbability)
            {
                output.Append(this.NextTrailingConsonantSequence());
            }

            return output.ToString();
        }
    }
}
