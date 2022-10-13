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
        /// Step 3 of X: Filter out three consonants in a row.
        /// </summary>
        public PlanetGeneratorV3()
        {
            this.UsingSyllableCount(2, 3);

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z");
            f.DoNotAllowPattern("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            this.UsingFilter(f);
        }

    }
}
