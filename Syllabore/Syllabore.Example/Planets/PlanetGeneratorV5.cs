using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV5 : NameGenerator
    {
        /// <summary>
        /// Step 5 of X: (Optional) Condense the filter
        /// </summary>
        public PlanetGeneratorV5()
        {
            this.UsingSyllableCount(2, 3);

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
