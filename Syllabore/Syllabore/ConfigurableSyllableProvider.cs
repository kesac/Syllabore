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
        private List<string> StartingConsonants { get; set; }
        private List<string> StartingConsonantSequences { get; set; }
        private List<string> Vowels { get; set; }
        private List<string> VowelSequences { get; set; }
        private List<string> EndingConsonants { get; set; }
        private List<string> EndingConsonantSequences { get; set; }

        public double StartingVowelProbability { get; set; }
        public double StartingConsonantSequenceProbability { get; set; }
        public double VowelSequenceProbability { get; set; }
        public double EndingConsonantProbability { get; set; }
        public double EndingConsonantSequenceProbability { get; set; }

        public ConfigurableSyllableProvider()
        {
            this.Random = new Random();

            this.StartingConsonants = new List<string>();
            this.StartingConsonantSequences = new List<string>();
            this.Vowels = new List<string>();
            this.VowelSequences = new List<string>();
            this.EndingConsonants = new List<string>();
            this.EndingConsonantSequences = new List<string>();

            this.StartingVowelProbability = 0.10;
            this.StartingConsonantSequenceProbability = 0.20;
            this.VowelSequenceProbability = 0.20;
            this.EndingConsonantProbability = 0.10;
            this.EndingConsonantSequenceProbability = 0.10;
        }

        /// <summary>
        /// Adds the specified consonant as a consonant that can occur before a vowel.
        /// </summary>
        public void AddStartingConsonant(string consonant)
        {
            this.StartingConsonants.Add(consonant);
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can occur before a vowel.
        /// </summary>
        public void AddStartingConsonant(string[] consonants)
        {
            this.StartingConsonants.AddRange(consonants);
        }

        /// <summary>
        /// Adds the specified consonant sequence as a sequence that can occur before a vowel.
        /// </summary>
        public void AddStartingConsonantSequence(string consonantSequence)
        {
            this.StartingConsonantSequences.Add(consonantSequence);
        }

        /// <summary>
        /// Adds the specified consonant sequences as sequences that can occur before a vowel.
        /// </summary>
        public void AddStartingConsonantSequence(string[] consonantSequences)
        {
            this.StartingConsonantSequences.AddRange(consonantSequences);
        }

        /// <summary>
        /// Adds the specified vowel as a vowel that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowel(string vowel)
        {
            this.Vowels.Add(vowel);
        }

        /// <summary>
        /// Adds the specified vowels as vowels that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowel(string[] vowels)
        {
            this.Vowels.AddRange(vowels);
        }

        /// <summary>
        /// Adds the specified vowel sequence as a sequence that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowelSequence(string vowelSequence)
        {
            this.VowelSequences.Add(vowelSequence);
        }

        /// <summary>
        /// Adds the specified vowel sequences as sequences that can be used to form the 'center' of syllables.
        /// </summary>
        public void AddVowelSequence(string[] vowelSequences)
        {
            this.VowelSequences.AddRange(vowelSequences);
        }

        /// <summary>
        /// Adds the specified consonant as a consonant that can appear after a vowel.
        /// </summary>
        public void AddEndingConsonant(string consonant)
        {
            this.EndingConsonants.Add(consonant);
        }

        /// <summary>
        /// Adds the specified consonants as consonants that can appear after a vowel.
        /// </summary>
        public void AddEndingConsonant(string[] consonants)
        {
            this.EndingConsonants.AddRange(consonants);
        }

        /// <summary>
        /// Adds the specified consonant sequence as a sequence that can appear after a vowel.
        /// </summary>
        public void AddEndingConsonantSequence(string consonantSequence)
        {
            this.EndingConsonantSequences.Add(consonantSequence);
        }

        /// <summary>
        /// Adds the specified consonant sequences as sequences that can appear after a vowel.
        /// </summary>
        public void AddEndingConsonantSequence(string[] consonantSequences)
        {
            this.EndingConsonantSequences.AddRange(consonantSequences);
        }

        private string NextConsonant()
        {
            return this.StartingConsonants[this.Random.Next(this.StartingConsonants.Count)];
        }

        private string NextConsonantSequence()
        {
            return this.StartingConsonantSequences[this.Random.Next(this.StartingConsonantSequences.Count)];
        }

        private string NextVowel()
        {
            return this.Vowels[this.Random.Next(this.Vowels.Count)];
        }

        private string NextVowelSequence()
        {
            return this.VowelSequences[this.Random.Next(this.VowelSequences.Count)];
        }

        private string NextEndingConsonant()
        {
            return this.EndingConsonants[this.Random.Next(this.EndingConsonants.Count)];
        }

        private string NextEndingConsonantSequence()
        {
            return this.EndingConsonantSequences[this.Random.Next(this.EndingConsonantSequences.Count)];
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

        private string GenerateSyllable(bool allowStartingVowel, bool allowSequences)
        {
            var output = new StringBuilder();

            if (allowStartingVowel && this.Random.NextDouble() < this.StartingVowelProbability)
            {
                output.Append(this.NextVowel());
            }
            else
            {

                if (allowSequences && this.Random.NextDouble() < this.StartingConsonantSequenceProbability)
                {
                    output.Append(this.NextConsonantSequence());
                }
                else
                {
                    output.Append(this.NextConsonant());
                }

                if (allowSequences && this.Random.NextDouble() < this.VowelSequenceProbability)
                {
                    output.Append(this.NextVowelSequence());
                }
                else
                {
                    output.Append(this.NextVowel());
                }
            }

            if (this.Random.NextDouble() < this.EndingConsonantProbability)
            {
                output.Append(this.NextEndingConsonant());
            }
            else if (allowSequences && this.Random.NextDouble() < this.EndingConsonantSequenceProbability)
            {
                output.Append(this.NextEndingConsonantSequence());
            }

            return output.ToString();
        }
    }
}
