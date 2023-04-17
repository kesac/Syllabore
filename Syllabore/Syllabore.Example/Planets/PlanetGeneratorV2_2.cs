using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2_2 : NameGenerator
    {

        public PlanetGeneratorV2_2()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");
            f.DoNotAllowPattern("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            this.UsingFilter(f);
        }

    }
}
