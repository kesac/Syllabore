using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Spaceship
{
    /// <summary>
    /// Building a name generator for spaceships.
    /// Part 1 of 5: Initializing a composite generator.
    /// </summary>
    public class SpaceshipGeneratorV1
    {
        private NameGenerator _prefixGenerator;
        private NameGenerator _shipGenerator;

        public SpaceshipGeneratorV1()
        {
            _prefixGenerator = new NameGenerator();
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
