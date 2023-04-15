using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{

    public class PlanetGeneratorV1 : NameGenerator
    {
        public PlanetGeneratorV1()
        {
            this.UsingSyllableCount(2, 3);

            var p = new SyllableProvider();
            p.WithVowels("aieou");
            p.WithConsonants("bcdfghjklmnpqrstvwxyz");

            this.UsingSyllables(p);

        }
    }
}
