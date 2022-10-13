using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{

    public class PlanetGeneratorV1_2 : NameGenerator
    {
        public PlanetGeneratorV1_2()
        {
            this.UsingSyllableCount(2, 3);

            var p = new SyllableProvider();
            p.WithVowels("aieou");
            p.WithLeadingConsonants("bcdfghklmnpqrstvxyz");
            p.WithTrailingConsonants("cdfgklmnprstv");
            p.WithProbability(x => x.TrailingConsonantExists(0.50));

            this.UsingProvider(p);

        }
    }
}
