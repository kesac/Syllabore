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
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1\1"))
                .UsingSyllableCount(3);

            _shipGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("aoi") // Don't need to separate vowels with commas if they're not sequences
                    .WithVowelSequences("ei", "ia", "ou", "eu") // For sequences though, we'll obviously need the commas
                    .WithLeadingConsonants("rstlmn").Weight(4) // Weights are relative to each other
                    .WithLeadingConsonants("cdgp").Weight(2)) // So this line says "2 out of 6" or 33% of the time
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1")) // Regex again, for two of the same letters consecutively
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
