using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example
{
    // Romanized Japanese-sounding names, maybe
    public class JPGenerator : NameGenerator
    {
        public JPGenerator()
        {
            this.UsingSyllables(x => x
                    .WithLeadingConsonants("khns").Weight(4)
                    .WithLeadingConsonants("rmyjd").Weight(2)
                    .WithLeadingConsonants("bgptwz")
                    .Sequences("sh")
                    .WithVowels("a").Weight(4)
                    .WithVowels("io").Weight(2)
                    .WithVowels("eu")
                    .WithProbability(x => x
                        .OfLeadingConsonants(1.0, 0.1)))
                .UsingTransform(x => x
                    .ReplaceAll("hu", "fu")
                    .ReplaceAll("si", "shi")
                    .ReplaceAll("ti", "chi")
                    .ReplaceAll("tu", "tsu"))
                .UsingFilter(x => x
                    .DoNotAllowEnding("[aeiou]{2}") // Avoids two-vowel endings
                    .DoNotAllowSubstring("yi", "ye", "wi", "wu", "we", "sf"))
                .UsingSyllableCount(2, 4);

            // Example output:
            // Zako
            // Hoyako
            // Nojayusha
            // Kashinoge
            // Shodoko

        }

    }
}
