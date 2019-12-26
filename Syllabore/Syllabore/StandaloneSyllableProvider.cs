using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// A quick and dirty standlone syllable provider for quick use. Custom providers
    /// should use <c>ConfigurableSyllableProvider</c> instead.
    /// </summary>
    public class StandaloneSyllableProvider : ConfigurableSyllableProvider
    {

        private static readonly string[] StartingConsonants = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };
        private static readonly string[] StartingConsonantSequences = { "CH", "SH", "BL", "CL", "FL", "PL", "GL", "BR", "CR", "DR", "PR", "TR", "TH", "SC", "SP", "ST", "SL", "SPR" };
        private static readonly string[] Vowels = { "A", "E", "I", "O", "U" };
        private static readonly string[] VowelSequences = { "AE", "EA", "AI", "IA", "AU", "AY", "IE", "OI", "OU", "EY", };
        private static readonly string[] EndingConsonants = { "B", "C", "D", "F", "G", "H", "K", "L", "M", "N", "P", "R", "S", "T", "V", "X", "Y"};
        private static readonly string[] EndingConsonantSequences = { "CK", "ST", "SC", "NG", "NK", "RSH", "LSH", "RK", "RST", "NCT", "XT" };

        public StandaloneSyllableProvider()
        {

            this.AddStartingConsonant(StartingConsonants);
            this.AddStartingConsonantSequence(StartingConsonantSequences);
            this.AddVowel(Vowels);
            this.AddVowelSequence(VowelSequences);
            this.AddEndingConsonant(EndingConsonants);
            this.AddEndingConsonantSequence(EndingConsonantSequences);

        }


    }
}
