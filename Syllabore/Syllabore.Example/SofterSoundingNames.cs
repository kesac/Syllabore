using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archigen;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    /// <summary>
    /// Names that are soft sounding when pronounced.
    /// </summary>
    public class SofterSoundingNames : Example
    {
        /// <summary>
        /// Generates names like: Lelia, Yannomo, Lammola
        /// </summary>
        public IGenerator<string> GetGenerator()
        {
            var names = new NameGenerator()
                .Start(x => x
                    .First(x => x
                        .Add("lmny").Weight(8)
                        .Add("wr").Weight(2)
                        .Add("s"))
                    .Middle(x => x
                        .Add("aeo").Weight(4)
                        .Add("u")
                        .Cluster("ia", "oe", "oi")))
                .Inner(x => x.CopyStart()
                    .First(x => x
                        .Cluster("mm", "nn", "mn", "ll")))
                .End(x => x.CopyStart()
                    .Last(x => x
                        .Add("smn")
                        .Cluster("sh", "th"))
                        .Chance(0.20))
                .SetSize(2, 3);

            return names;
        }
    }
}
