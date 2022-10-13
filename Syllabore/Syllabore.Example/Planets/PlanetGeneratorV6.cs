using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV6 : NameGenerator
    {
        /// <summary>
        /// Step 6 of X: (Optional) Weight the consonant and vowel pool
        /// </summary>
        public PlanetGeneratorV6()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingProvider(x => x
                .WithConsonants("bcdfghjklmnpqrstvwxyz")
                .WithVowels("aioue"));

            this.UsingFilter(x => x
                .DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z")
                .DoNotAllowPattern("([^aieou]{3})")
                .DoNotAllowPattern("(q[^u])")
                .DoNotAllowPattern("([^tsao]w)")
                .DoNotAllow("pn", "zz", "yy", "xx")
                .DoNotAllowPattern("(y[^aeiou])")
                .DoNotAllowPattern("(p[^aeioustrlh])"));
        }

    }
}
