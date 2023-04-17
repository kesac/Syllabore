using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Spaceship
{
    /// <summary>
    /// Building a name generator for spaceships.
    /// Part 2 of 5: Shaping the prefix.
    /// </summary>
    public class SpaceshipGeneratorV2
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        public SpaceshipGeneratorV2()
        {
            _prefixGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithConsonants("UVXSHMLAMN")
                    .WithProbability(x => x
                        .OfVowels(0.0)
                        .OfLeadingConsonants(1.0)
                        .OfTrailingConsonants(0.0)));

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
