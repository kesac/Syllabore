﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Spaceship
{
    public class SpaceshipGeneratorV5
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        /// <summary>
        /// Building a name generator for spaceships.
        /// Part 5 of 5: Adding specific suffixes.
        /// </summary>
        public SpaceshipGeneratorV5()
        {
            _prefixGenerator = new NameGenerator()
                .UsingProvider(x => x
                    .WithConsonants("SHMLAMN").Weight(1)
                    .WithConsonants("UVX").Weight(2)
                    .WithProbability(x => x
                        .VowelExists(0.0)
                        .LeadingConsonantExists(1.0)
                        .TrailingConsonantExists(0.0)))
                .UsingFilter(x => x
                    .DoNotAllowPattern(@"(\w)\1\1"))
                .UsingSyllableCount(3);

            _shipGenerator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("aoi")
                    .WithVowelSequences("ei", "ia", "ou", "eu")
                    .WithLeadingConsonants("rstlmn").Weight(4)
                    .WithLeadingConsonants("cdgp").Weight(2))
                .UsingTransformer(x => x
                    .Select(1) // Only apply one transform
                    .Chance(0.50) // Only allow transform to be used 50% of the time
                    .WithTransform(x => x.ReplaceSyllable(-1, "des")) // Index -1 is the last position
                    .WithTransform(x => x.ReplaceSyllable(-1, "rus"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "vium")))
                .UsingFilter(x => x
                    .DoNotAllowPattern(@"(\w)\1"))
                .UsingSyllableCount(3);
        }

        public string Next()
        {
            var prefix = _prefixGenerator.Next().ToUpper();
            var ship = _shipGenerator.Next();

            return String.Format("{0} {1}", prefix, ship);
        }

    }
}