using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.RandomString
{
    public class RandomTextGenerator : NameGenerator
    {
        private const string AlphanumericCharacters = "abcdefghijklmnopqrstuvwxyz0123456789";

        public RandomTextGenerator()
        {
            var syllables = new SyllableGenerator()
                .WithConsonants(AlphanumericCharacters)
                .WithVowels(AlphanumericCharacters);
            this.UsingFilter(new NameFilter());
            this.UsingSyllables(syllables);

            this.UsingSyllableCount(8);

        }
    }
}
