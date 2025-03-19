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
    /// Names that sound harsh when pronounced.
    /// </summary>
    public class HarshSoundingNames : Example
    {
        /// <summary>
        /// Generates names like: Batozk, Tobarg, Tega'pezk
        /// </summary>
        public IGenerator<string> GetGenerator()
        {
            var names = new NameGenerator()
                .Start(x => x
                    .First("kgtdpb")
                    .Middle(x => x
                        .Add("aou").Weight(4)
                        .Add("e").Weight(1)))
                .Inner(x => x
                    .CopyStart()
                    .Last(x => x
                        .Add("ckg'")))
                .End(x => x
                    .CopyStart()
                    .Last(x => x
                        .Cluster("ck", "zk", "rg")))
                .SetSize(2, 3);

            return names;
        }
    }
}
