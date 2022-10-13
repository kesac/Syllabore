using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV7 : NameGenerator
    {
        /// <summary>
        /// Step 7 of X: (Optional) Weight the consonant and vowel pool
        /// </summary>
        public PlanetGeneratorV7()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingProvider(x => x
                .WithConsonants("bcdhlmnprst").Weight(4)
                .WithConsonants("gfjk").Weight(2)
                .WithConsonants("vqwxyz").Weight(1)
                .WithVowels("e").Weight(3)
                .WithVowels("ai").Weight(2)
                .WithVowels("ou").Weight(1));

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
