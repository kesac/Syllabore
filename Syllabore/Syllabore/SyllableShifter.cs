using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public class SyllableShifter : IShifter
    {
        private ISyllableProvider Provider { get; set; }

        private Random Random { get; set; }

        public SyllableShifter(ISyllableProvider provider)
        {
            this.Provider = provider ?? throw new ArgumentNullException("The specified ISyllableProvider is null.");
            this.Random = new Random();
        }

        public Name NextVariation(Name sourceName)
        {
            var syllables = new string[sourceName.Syllables.Length];
            Array.Copy(sourceName.Syllables, syllables, sourceName.Syllables.Length);

            Name result = new Name(syllables);
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
