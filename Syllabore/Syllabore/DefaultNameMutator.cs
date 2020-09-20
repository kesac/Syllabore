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
    public class DefaultNameMutator : ConfigurableNameMutator
    {
        private ISyllableProvider Provider { get; set; }

        private Random Random { get; set; }

        public DefaultNameMutator()
        {
            this.Provider = new DefaultSyllableProvider();
            this.Random = new Random();

            this.WithMutation(name => {

                int index = this.Random.Next(name.Syllables.Count);

                if (index == 0)
                {
                    name.Syllables[index] = this.Provider.NextStartingSyllable();
                }
                else if (index == name.Syllables.Count - 1)
                {
                    name.Syllables[index] = this.Provider.NextEndingSyllable();
                }
                else
                {
                    name.Syllables[index] = this.Provider.NextSyllable();
                }

            });

        }

    }
}
