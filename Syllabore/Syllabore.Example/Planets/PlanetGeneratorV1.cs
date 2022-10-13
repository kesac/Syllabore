using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    /// <summary>
    /// Step 1 of X: Extend from the NameGenerator class.
    /// </summary>
    public class PlanetGeneratorV1 : NameGenerator
    {
        public PlanetGeneratorV1()
        {
            this.UsingSyllableCount(2, 3);
        }
    }
}
