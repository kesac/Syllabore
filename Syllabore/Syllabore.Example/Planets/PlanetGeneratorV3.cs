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

            this.UsingProvider(x => x
                .WithVowels("aie").Weight(2)
                .WithVowels("ou").Weight(1)
                .WithLeadingConsonants("bcdfghlmnprstvy").Weight(2)
                .WithLeadingConsonants("qkxz").Weight(1)
                .WithTrailingConsonants("dlmnprst").Weight(2)
                .WithTrailingConsonants("cdfkvg").Weight(1)
                .WithProbability(x => x.TrailingConsonantExists(0.50)));

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
