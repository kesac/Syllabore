using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    // This is meant to be a standlone syllable model for quick use
    public class BasicSyllableModel : ISyllableModel
    {

        private static readonly string[] Consonants = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };
        private static readonly string[] ConsonantClusters = { "CH", "SH", "BL", "CL", "FL", "PL", "GL", "BR", "CR", "DR", "PR", "TR", "TH", "SC", "SP", "ST", "SL", "SPR" };
        private static readonly string[] Vowels = { "A", "E", "I", "O", "U" };
        private static readonly string[] VowelClusters = { "AE", "EA", "AI", "IA", "AU", "AY", "IE", "OI", "OU", "EY" };
        private static readonly string[] Codas = { "C", "D", "F", "G", "NG", "H", "CK", "L", "M", "M", "R", "S", "T", "ST", "SC" };

        private Random Random { get; set; }

        public double ConsonantClusterProbability { get; set; }
        public double VowelClusterProbability { get; set; }
        public double CodaProbability { get; set; }

        public BasicSyllableModel()
        {
            this.Random = new Random();
            this.ConsonantClusterProbability = 0.2;
            this.VowelClusterProbability = 0.2;
            this.CodaProbability = 0.2;
        }

        public string NextConsonant()
        {
            return Consonants[this.Random.Next(Consonants.Length)];
        }

        public string NextConsonantCluster()
        {
            return ConsonantClusters[this.Random.Next(ConsonantClusters.Length)];
        }

        public string NextVowel()
        {
            return Vowels[this.Random.Next(Vowels.Length)];
        }

        public string NextVowelCluster()
        {
            return VowelClusters[this.Random.Next(VowelClusters.Length)];
        }

        public string NextCoda()
        {
            return Codas[this.Random.Next(Codas.Length)];
        }

        public string NextSyllable()
        {

            var output = new StringBuilder();
            if (this.Random.NextDouble() < this.ConsonantClusterProbability)
            {
                output.Append(this.NextConsonantCluster());
            }
            else
            {
                output.Append(this.NextConsonant());
            }

            if (this.Random.NextDouble() < this.VowelClusterProbability)
            {
                output.Append(this.NextVowelCluster());
            }
            else
            {
                output.Append(this.NextVowel());
            }
            

            if (this.Random.NextDouble() < this.CodaProbability)
            {
                output.Append(this.NextCoda());
            }

            return output.ToString();
        }
    }
}
