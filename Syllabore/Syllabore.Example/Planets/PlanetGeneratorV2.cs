using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2 : NameGenerator
    {
        /// <summary>
        /// Step 2 of X: Filter out awkward looking endings.
        /// </summary>
        public PlanetGeneratorV2()
        {
            this.UsingSyllableCount(2, 3);

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z");

            this.UsingFilter(f);
        }

    }
}
