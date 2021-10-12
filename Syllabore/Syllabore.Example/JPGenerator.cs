﻿using System;
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
            this.UsingProvider(x => x
                    .WithLeadingConsonants("khns").Weight(4)
                    .WithLeadingConsonants("rmyjd").Weight(2)
                    .WithLeadingConsonants("bgptwz")
                    .Sequences("sh")
                    .WithVowels("a").Weight(4)
                    .WithVowels("io").Weight(2)
                    .WithVowels("eu")
                    .WithProbability(x => x
                        .LeadingConsonantBecomesSequence(0.1)
                        .StartingSyllable.LeadingVowelExists(0.20)))
                .UsingMutator(x => x
                    .WithMutation(x => x
                        .ReplaceAll("hu", "fu").ReplaceAll("si", "shi")
                        .ReplaceAll("ti", "chi").ReplaceAll("tu", "tsu")))
                .UsingValidator(x => x
                    .DoNotAllowEnding("[aeiou]{2}") // Avoids two-vowel endings
                    .DoNotAllowPattern("yi", "ye", "wi", "wu", "we", "sf"))
                .LimitMutationChance(1.0)
                .LimitSyllableCount(2, 4);

            // Example output:
            // Zako
            // Hoyako
            // Nojayusha
            // Kashinoge
            // Shodoko

        }

    }
}
