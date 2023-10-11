using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Spaceship
{
    /// <summary>
    /// Building a name generator for spaceships.
    /// Part 3 of 5: Improving the prefix output.
    /// </summary>
    public class SpaceshipGeneratorV3
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        public SpaceshipGeneratorV3()
        { 
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithConsonants("SHMLAMN").Weight(1)
                    .WithConsonants("UVX").Weight(2) // These letters will appear twice more likely than others
                    .WithProbability(x => x
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)))
                .UsingFilter(x => x
                    .DoNotAllow(@"(\w)\1\1")) // Regex for three consecutive same letters
                .UsingSyllableCount(3); // In our case, this changes prefix length, not syllable count

            _shipGenerator = new NameGenerator();
        }

        public string Next()
        {
            var prefix = _prefixGenerator.Next().ToUpper();
            var ship = _shipGenerator.Next();

            return String.Format("{0} {1}", prefix, ship);
        }

    }
}
