using System;
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
                .UsingSyllables(x => x
                    .WithConsonants("SHMLAMN").Weight(1)
                    .WithConsonants("UVX").Weight(2)
                    .WithProbability(x => x
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1\1"))
                .UsingSyllableCount(3);

            _shipGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("aoi")
                    .WithVowelSequences("ei", "ia", "ou", "eu")
                    .WithLeadingConsonants("rstlmn").Weight(4)
                    .WithLeadingConsonants("cdgp").Weight(2))
                .UsingTransform(0.5, new TransformSet() // Only allow transform to be used 50% of the time
                    .RandomlySelect(1) // Only apply one transform
                    .WithTransform(x => x.ReplaceSyllable(-1, "des")) // Index -1 is the last position
                    .WithTransform(x => x.ReplaceSyllable(-1, "rus"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "vium")))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1"))
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
