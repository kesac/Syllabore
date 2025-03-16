using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example
{
    /// <summary>
    /// Generates names that sound similar to each other.
    /// This is done by restricting the number of unique syllables possible
    /// through a <see cref="SyllableSet"/>.
    /// </summary>
    public class SimilarSoundingNames
    {
        public NameGenerator GetNonFluentGenerator()
        {
            var syllables = new SyllableGenerator()
                .Add(SymbolPosition.First, "bgjdktplmnrs")
                .Add(SymbolPosition.Middle, "aeiou");

            var restrictedSyllables = new SyllableSet(syllables, 8, true);

            var names = new NameGenerator()
                .SetSyllables(SyllablePosition.Any, restrictedSyllables)
                .SetSize(3,4);

            return names;
        }
    }
}
