using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Creates variations on names by replacing one syllable
    /// with another syllable. This class uses the DefaultSyllableProvider
    /// by default.
    /// </summary>
    public class DefaultNameMutator : INameMutator
    {
        private ISyllableProvider Provider { get; set; }

        private Random Random { get; set; }

        public DefaultNameMutator()
        {
            this.Provider = new DefaultSyllableProvider();
            this.Random = new Random();
        }

        public Name Mutate(Name sourceName)
        {
            Name result = new Name(new string[sourceName.Syllables.Length]);            
            Array.Copy(sourceName.Syllables, result.Syllables, sourceName.Syllables.Length);

            int index = this.Random.Next(sourceName.Syllables.Length);

            if (index == 0)
            {
                result.Syllables[index] = this.Provider.NextStartingSyllable();
            }
            else if (index == sourceName.Syllables.Length - 1)
            {
                result.Syllables[index] = this.Provider.NextEndingSyllable();
            }
            else
            {
                result.Syllables[index] = this.Provider.NextSyllable();
            }

            return result;
        }
    }
}
