using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    
    /// Experimental: finds vowels in a name then changes it to another vowel
    public class VowelMutator : IMutator
    {
        private static readonly string[] DefaultVowels = { "a", "e", "i", "o", "u" };
        private List<string> VowelPool { get; set; }
        private Random Random { get; set; }

        public VowelMutator() : this(DefaultVowels) { }

        public VowelMutator(params string[] vowels)
        {

            this.VowelPool = new List<string>();

            foreach(var v in vowels)
            {
                this.VowelPool.AddRange(v.Atomize());
            }

            this.Random = new Random();
        }

        /*
        public VowelShifter UsingVowel(params string[] vowel)
        {
            foreach(var v in vowel)
            {
                this.VowelPool.Add(v);
            }

            return this;
        }
        */

        /// TODO this is only shifting vowels right now and it doesn't handle vowel sequences
        public Name Mutate(Name sourceName)
        {
            Name result = new Name(sourceName);
            int index = this.Random.Next(sourceName.Syllables.Count);
            var syllable = result.Syllables[index];
            result.Syllables[index] = Regex.Replace(syllable, "([aeiouAEIOU]+)", this.VowelPool[this.Random.Next(this.VowelPool.Count)]);

            return result;
        }
    }
}
