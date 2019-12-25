using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // This is meant to be a standlone syllable model for quick use
    public class BasicSyllableProvider : IProvider
    {

        private static readonly string[] StartingConsonants = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };
        private static readonly string[] StartingConsonantSequences = { "CH", "SH", "BL", "CL", "FL", "PL", "GL", "BR", "CR", "DR", "PR", "TR", "TH", "SC", "SP", "ST", "SL", "SPR" };
        private static readonly string[] Vowels = { "A", "E", "I", "O", "U" };
        private static readonly string[] VowelSequences = { "AE", "EA", "AI", "IA", "AU", "AY", "IE", "OI", "OU", "EY", };
        private static readonly string[] EndingConsonants = { "B", "C", "D", "F", "G", "H", "K", "L", "M", "N", "P", "R", "S", "T", "V", "X", "Y"};
        private static readonly string[] EndingConsonantSequences = { "CK", "ST", "SC", "NG", "NK", "RSH", "LSH", "RK", "RST", "NCT", "XT" };

        private Random Random { get; set; }

        public double StartingVowelProbability { get; set; }
        public double StartingConsonantSequenceProbability { get; set; }
        public double VowelSequenceProbability { get; set; }
        public double EndingConsonantProbability { get; set; }
        public double EndingConsonantSequenceProbability { get; set; }

        public BasicSyllableProvider()
        {
            this.Random = new Random();
            this.StartingVowelProbability = 0.10;
            this.StartingConsonantSequenceProbability = 0.20;
            this.VowelSequenceProbability = 0.20;
            this.EndingConsonantProbability = 0.10;
            this.EndingConsonantSequenceProbability = 0.10;
        }

        private string NextConsonant()
        {
            return StartingConsonants[this.Random.Next(StartingConsonants.Length)];
        }

        private string NextConsonantSequence()
        {
            return StartingConsonantSequences[this.Random.Next(StartingConsonantSequences.Length)];
        }

        private string NextVowel()
        {
            return Vowels[this.Random.Next(Vowels.Length)];
        }

        private string NextVowelSequence()
        {
            return VowelSequences[this.Random.Next(VowelSequences.Length)];
        }

        private string NextEndingConsonant()
        {
            return EndingConsonants[this.Random.Next(EndingConsonants.Length)];
        }

        private string NextEndingConsonantSequence()
        {
            return EndingConsonantSequences[this.Random.Next(EndingConsonantSequences.Length)];
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
            else {

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
            else if(allowSequences && this.Random.NextDouble() < this.EndingConsonantSequenceProbability)
            {
                output.Append(this.NextEndingConsonantSequence());
            }

            return output.ToString();
        }

    }
}
