using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// TODO add vowel shifting and consonant shifting
    public class LetterShifter : IShifter
    {
        private List<string> VowelPool { get; set; }
        private List<string> StartingConsonantPool { get; set; }
        private List<string> EndingConsonantPool { get; set; }

        private Random Random { get; set; }

        public LetterShifter(ConfigurableSyllableProvider provider)
        {
            if (provider == null) {
                throw new ArgumentNullException("The specified ISyllableProvider is null.");
            }

            this.VowelPool = provider.GetAllVowels();
            this.StartingConsonantPool = provider.GetAllStartingConsonants();
            this.EndingConsonantPool = provider.GetAllEndingConsonants();

            this.Random = new Random();
        }

        /// TODO this is only shifting vowels right now and it doesn't handle vowel sequences
        public Name NextVariation(Name sourceName)
        {
            var syllables = new string[sourceName.Syllables.Length];
            Array.Copy(sourceName.Syllables, syllables, sourceName.Syllables.Length);

            Name result = new Name(syllables);


            int index = this.Random.Next(sourceName.Syllables.Length);

            var syllable = result.Syllables[index];

            result.Syllables[index] = Regex.Replace(syllable, "([aeiouAEIOU])", this.VowelPool[this.Random.Next(this.VowelPool.Count)]);


            return result;
        }
    }
}
