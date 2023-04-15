using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Spaceship
{
    public class SpaceshipGeneratorV4
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        /// <summary>
        /// Building a name generator for spaceships.
        /// Part 4 of 5: Shaping the ship's name.
        /// </summary>
        public SpaceshipGeneratorV4()
        {
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
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
                .UsingSyllables(x => x
                    .WithVowels("aoi")
                    .WithVowelSequences("ei", "ia", "ou", "eu")
                    .WithLeadingConsonants("rstlmn").Weight(4)
                    .WithLeadingConsonants("cdgp").Weight(2))
                .UsingFilter(x => x
                    .DoNotAllowPattern(@"(\w)\1")) // Two of the same letters consecutively
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
