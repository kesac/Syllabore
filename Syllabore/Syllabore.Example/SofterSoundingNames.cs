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

        /// <summary>
        /// Same as GetGenerator(), but without using Syllabore.Fluent extension methods
        /// </summary>
        public IGenerator<string> GetNonFluentGenerator()
        {
            var startingConsonants = new SymbolGenerator()
                .Add("lmny").Weight(8)
                .Add("wr").Weight(2)
                .Add("s");

            var vowels = new SymbolGenerator()
                .Add("aeo").Weight(4)
                .Add("u")
                .Cluster("ia", "oe", "oi");

            var innerConsonants = new SymbolGenerator()
                .Cluster("mm", "nn", "mn", "ll");

            var endingConsonants = new SymbolGenerator()
                .Add("smn")
                .Cluster("sh", "th");

            var startingSyllables = new SyllableGenerator()
                .Add(SymbolPosition.First, startingConsonants)
                .Add(SymbolPosition.Middle, vowels);

            var innerSyllables = (SyllableGenerator)startingSyllables.Copy();
            innerSyllables.Add(SymbolPosition.First, innerConsonants);

            var endingSyllables = (SyllableGenerator)startingSyllables.Copy();
            endingSyllables.Add(SymbolPosition.Last, endingConsonants);
            endingSyllables.SetChance(SymbolPosition.Last, 0.20);

            var names = new NameGenerator()
                .SetSyllables(SyllablePosition.Starting, startingSyllables)
                .SetSyllables(SyllablePosition.Inner, innerSyllables)
                .SetSyllables(SyllablePosition.Ending, endingSyllables)
                .SetSize(2, 3);

            return names;
        }

    }
}
