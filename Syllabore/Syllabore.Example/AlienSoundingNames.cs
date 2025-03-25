using Archigen;
using Syllabore.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example
{
    /// <summary>
    /// Names that sound like they could be from an alien species.
    /// </summary>
    public class AlienSoundingNames : Example
    {
        /// <summary>
        /// Names like: "Xal-Kithath", "Z'nuth", "Yommur"
        /// </summary>
        public IGenerator<string> GetGenerator()
        {
            var result = new GeneratorPool<string>();
            result.Add(GetSimpleNameGenerator(2));
            result.Add(GetApostropheNameGenerator());
            result.Add(GetDashedNameGenerator());

            return result;
        }

        /// <summary>
        /// Names like "Xuul-Tizdu", "Zal-Tathkuth", "Xam-Gidu"
        /// </summary>
        public IGenerator<string> GetDashedNameGenerator()
        {
            var names = new NameGenerator()
                .Any(x => x
                    .First(x => x
                        .Add("ngdkt").Weight(2)
                        .Cluster("th", "ts", "qu"))
                    .Middle(x => x
                        .Add("aiou").Weight(3)
                        .Add("e"))
                    .Last(x => x
                        .Add("z")
                        .Cluster("th").Weight(2))
                        .Chance(0.1))
                .SetSize(2);

            var result = new NameFormatter("{start}-{end}");
            result.Define("start", GetSimpleNameGenerator(1));
            result.Define("end", names);

            return result;
        }

        /// <summary>
        /// Generates names like: "Za'knuk", "Y'thut", "Xo'tsuuthtsuk"
        /// </summary>
        public IGenerator<string> GetApostropheNameGenerator()
        {
            var front = new NameGenerator()
                .Any(x => x
                    .First("kxyz")
                    .Middle("oai").Chance(0.5))
                .SetSize(1);

            var back = new NameGenerator()
                .Any(x => x
                    .First(x => x
                        .Add("ngd").Weight(2)
                        .Cluster("th", "ts", "qu", "kn"))
                    .Middle(x => x
                        .Add("ua").Weight(4)
                        .Cluster("uu", "aa"))
                    .Last(x => x
                        .Add("rgkt")
                        .Cluster("th"))
                        .Chance(0.9))
                .Filter("gqu")
                .SetSize(1, 2);

            var result = new NameFormatter("{front}'{back}");
            result.Define("front", front);
            result.Define("back", back, NameFormat.LowerCase);

            return result;
        }

        /// <summary>
        /// Generates names like: "Xog", "Yalzath", and "Xaathath"
        /// </summary>
        public IGenerator<string> GetSimpleNameGenerator(int size)
        {
            return new NameGenerator()
                .Any(x => x
                    .First("xyz").Chance(0.9)
                    .Middle(x => x
                        .Add("oai").Weight(2)
                        .Cluster("uu", "aa"))
                    .Last(x => x
                        .Add("lgrm")
                        .Cluster("th", "gg")))
                .Filter("^aa", "^uu")
                .SetSize(size);
        }
    }
}
