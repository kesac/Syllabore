using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    public class HarshSoundingNames
    {
        /// <summary>
        /// Generates names like: Batozk, Tobarg, Tega'pezk
        /// </summary>
        public NameGenerator GetGenerator()
        {
            var names = new NameGenerator()
                .Lead(x => x
                    .First("kgtdpb")
                    .Middle(x => x
                        .Add("aou").Weight(4)
                        .Add("e").Weight(1)))
                .Inner(x => x
                    .CopyLead()
                    .Last(x => x
                        .Add("ckg'")))
                .Trail(x => x
                    .CopyLead()
                    .Last(x => x
                        .Cluster("ck", "zk", "rg")))
                .SetSize(2, 3);

            return names;
        }
    }
}
