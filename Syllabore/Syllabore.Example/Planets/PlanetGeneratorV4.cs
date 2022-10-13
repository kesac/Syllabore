using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV4 : NameGenerator
    {
        /// <summary>
        /// Step 4 of X: Filter out a few patterns for aesthetic reasons.
        /// </summary>
        public PlanetGeneratorV4()
        {
            this.UsingSyllableCount(2, 3);

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z");
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
