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
            var provider = new SyllableGenerator()
                .WithConsonants(AlphanumericCharacters)
                .WithVowels(AlphanumericCharacters);

            this.UsingSyllables(provider);

            this.UsingSyllableCount(8);

        }
    }
}
