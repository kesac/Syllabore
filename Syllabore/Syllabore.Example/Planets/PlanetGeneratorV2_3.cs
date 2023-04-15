using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2_3 : NameGenerator
    {

        public PlanetGeneratorV2_3()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.TrailingConsonantExists(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");
            f.DoNotAllowPattern("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            f.DoNotAllowPattern("(q[^u])"); // Q must always be followed by a u
            f.DoNotAllowPattern("([^tsao]w)"); // W must always be preceded with a t, s, a, or o

            // Some awkward looking combinations
            f.DoNotAllow("pn", "zz", "yy", "xx");
            f.DoNotAllowPattern("(y[^aeiou])"); // Avoids things like yt, yw, yz, etc.
            f.DoNotAllowPattern("(p[^aeioustrlh])"); // Avoids things like pb, pq, pz, etc.

            this.UsingFilter(f);
        }

    }
}
