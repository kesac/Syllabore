using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2_4 : NameGenerator
    {

        public PlanetGeneratorV2_4()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            this.UsingFilter(x => x
                .DoNotAllowEnding("f", "g", "j", "v")
                .DoNotAllowPattern("([^aieou]{3})")
                .DoNotAllowPattern("(q[^u])")
                .DoNotAllowPattern("([^tsao]w)")
                .DoNotAllow("pn", "zz", "yy", "xx")
                .DoNotAllowPattern("(y[^aeiou])")
                .DoNotAllowPattern("(p[^aeioustrlh])"));
        }

    }
}
