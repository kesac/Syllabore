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

            var s = new SyllableGenerator()
                    .WithVowels("aieou")
                    .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                    .WithTrailingConsonants("cdfgklmnprstv")
                    .WithProbability(x => x.OfTrailingConsonants(0.50));

            this.UsingSyllables(s);

        }
    }
}
