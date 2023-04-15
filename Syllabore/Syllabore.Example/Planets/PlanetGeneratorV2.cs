using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2 : NameGenerator
    {

        public PlanetGeneratorV2()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.TrailingConsonantExists(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");

            this.UsingFilter(f);
        }

    }
}
