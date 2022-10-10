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
        /// Part 5 of 5: Adding an optional transform.
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
                    .WithVowels("aeyo")
                    .WithVowelSequences("ei", "ia")
                    .WithLeadingConsonants("dfghlmnrstv"))
                .UsingTransformer(x => x
                    .Chance(0.50)
                    .WithTransform(x => x.ReplaceSyllable(-1, "des"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "rus"))
                    .WithTransform(x => x.ReplaceSyllable(-1, "vium")))
                .UsingSyllableCount(2, 4);
        }

        public string Next()
        {
            var prefix = _prefixGenerator.Next().ToUpper();
            var ship = _shipGenerator.Next();

            return String.Format("{0} {1}", prefix, ship);
        }

    }
}
