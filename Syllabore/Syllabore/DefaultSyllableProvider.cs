using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// The default syllable provider used by a vanilla instance
    /// of <see cref="NameGenerator"/>.
    /// </para>
    /// <para>
    /// All graphemes of the English language are used in this syllable provider.
    /// </para>
    /// </summary>
    public class DefaultSyllableProvider : SyllableProvider
    {

        private static readonly string[] DefaultLeadingConsonants = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };
        private static readonly string[] DefaultLeadingConsonantSequences = { "CH", "SH", "BL", "CL", "FL", "PL", "GL", "BR", "CR", "DR", "PR", "TR", "TH", "SC", "SP", "ST", "SL", "SPR" };
        private static readonly string[] DefaultVowels = { "A", "E", "I", "O", "U" };
        private static readonly string[] DefaultVowelSequences = { "AE", "EA", "AI", "IA", "AU", "AY", "IE", "OI", "OU", "EY" };
        private static readonly string[] DefaultTrailingConsonants = { "B", "C", "D", "F", "G", "H", "K", "L", "M", "N", "P", "R", "S", "T", "V", "X", "Y"};
        private static readonly string[] DefaultTrailingConsonantSequences = { "CK", "ST", "SC", "NG", "NK", "RSH", "LSH", "RK", "RST", "NCT", "XT" };

        public DefaultSyllableProvider()
        {
            this.WithLeadingConsonants(DefaultLeadingConsonants)
                .WithLeadingConsonantSequences(DefaultLeadingConsonantSequences)
                .WithVowels(DefaultVowels)
                .WithVowelSequences(DefaultVowelSequences)
                .WithTrailingConsonants(DefaultTrailingConsonants)
                .WithTrailingConsonantSequences(DefaultTrailingConsonantSequences);
        }


    }
}
