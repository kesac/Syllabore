using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    public class SofterSoundingNames
    {
        /// <summary>
        /// Generates names like: Lelia, Yannomo, Lammola
        /// </summary>
        public NameGenerator GetGenerator()
        {
            var names = new NameGenerator()
                .Lead(x => x
                    .First(x => x
                        .Add("lmny").Weight(8)
                        .Add("wr").Weight(2)
                        .Add("s"))
                    .Middle(x => x
                        .Add("aeo").Weight(4)
                        .Add("u")
                        .Cluster("ia", "oe", "oi")))
                .Inner(x => x.CopyLead()
                    .First(x => x
                        .Cluster("mm", "nn", "mn", "ll")))
                .Trail(x => x.CopyLead()
                    .Last(x => x
                        .Add("smn")
                        .Cluster("sh", "th"))
                        .Chance(0.20))
                .SetSize(2, 3);

            return names;
        }
    }
}
