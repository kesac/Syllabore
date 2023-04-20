using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV3 : NameGenerator
    {
        /// <summary>
        /// Step 5 of X: (Optional) Condense the filter
        /// </summary>
        public PlanetGeneratorV3()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aie").Weight(2)
                .WithVowels("ou").Weight(1)
                .WithLeadingConsonants("bcdfghlmnprstvy").Weight(2)
                .WithLeadingConsonants("qkxz").Weight(1)
                .WithTrailingConsonants("dlmnprst").Weight(2)
                .WithTrailingConsonants("cdfkvg").Weight(1)
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            this.UsingFilter(x => x
                .DoNotAllowEnding("f", "g", "j", "v")
                .DoNotAllow("([^aieou]{3})")
                .DoNotAllow("(q[^u])")
                .DoNotAllow("([^tsao]w)")
                .DoNotAllowSubstring("pn", "zz", "yy", "xx")
                .DoNotAllow("(y[^aeiou])")
                .DoNotAllow("(p[^aeioustrlh])"));
        }

    }
}
