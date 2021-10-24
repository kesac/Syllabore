using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// This is the default mutator that <see cref="NameGenerator"/> uses 
    /// when a custom mutator is not specified during instantiation.
    /// This mutator creates variations of names by replacing one syllable
    /// with another syllable. Syllables are derived from <see cref="DefaultSyllableProvider"/>.
    /// </summary>
    public class DefaultNameTransformer : NameTransformer
    {
        private ISyllableProvider Provider { get; set; }

        private Random Random { get; set; }

        public DefaultNameTransformer()
        {
            this.Provider = new DefaultSyllableProvider();
            this.Random = new Random();

            this.WithTransform(x => x
                .ExecuteUnserializableAction(name => {
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
            }));

        }

    }
}
