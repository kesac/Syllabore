using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    
    /// Experimental: finds vowels in a name then changes it to another vowel
    public class VowelMutator : NameMutator
    {
        private static readonly string[] DefaultVowels = { "a", "e", "i", "o", "u" };
        private List<string> VowelPool { get; set; }
        private Random Random { get; set; }

        public VowelMutator() : this(DefaultVowels) { }

        public VowelMutator(params string[] vowels)
        {

            this.Random = new Random();
            this.VowelPool = new List<string>();

            foreach(var v in vowels)
            {
                this.VowelPool.AddRange(v.Atomize());
            }

            this.WithMutation(x => x
                .ExecuteUnserializableAction(name => 
                {
                    int index = this.Random.Next(name.Syllables.Count);
                    var syllable = name.Syllables[index];
                    name.Syllables[index] = Regex.Replace(syllable, "([aeiouAEIOU]+)", this.VowelPool[this.Random.Next(this.VowelPool.Count)]);
                }));
        }

    }
}
