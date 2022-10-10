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
