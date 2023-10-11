using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example
{
    public class WeightedGenerator : NameGenerator
    {

        public WeightedGenerator()
        {
            var consonants = new []
            {
                new { Letter = "b", Weight = 100  },
                new { Letter = "c", Weight = 100  },
                new { Letter = "d", Weight = 250  },
                new { Letter = "f", Weight = 100  },
                new { Letter = "g", Weight = 50   },
                new { Letter = "h", Weight = 250  },
                new { Letter = "j", Weight = 10   },
                new { Letter = "k", Weight = 50   },
                new { Letter = "l", Weight = 500  },
                new { Letter = "m", Weight = 500  },
                new { Letter = "n", Weight = 500  },
                new { Letter = "p", Weight = 100  },
                new { Letter = "q", Weight = 10   },
                new { Letter = "r", Weight = 1000 },
                new { Letter = "s", Weight = 2000 },
                new { Letter = "t", Weight = 2000 },
                new { Letter = "v", Weight = 10  },
                new { Letter = "w", Weight = 10   },
                new { Letter = "x", Weight = 10   },
                new { Letter = "z", Weight = 10   }
            }; 

            var vowels = new[]
            {
                new { Letter = "a", Weight = 1000 },
                new { Letter = "e", Weight = 1000 },
                new { Letter = "i", Weight = 1000  },
                new { Letter = "o", Weight = 1000  },
                new { Letter = "y", Weight = 50  },
                new { Letter = "u", Weight = 50 }
            };

            var syllables = new SyllableGenerator();

            foreach(var consonant in consonants)
            {
                syllables.WithConsonants(consonant.Letter).Weight(consonant.Weight);
            }

            foreach (var vowel in vowels)
            {
                syllables.WithVowels(vowel.Letter).Weight(vowel.Weight);
            }

            this.UsingSyllables(syllables);
            
            this.UsingSyllableCount(2, 3);

            this.UsingProbability(x => x
                .OfTrailingConsonants(0.33)
                .OfLeadingVowelsInStartingSyllable(0.25)
                .OfLeadingConsonants(1.0)
            );

            this.DoNotAllow(
                "fd", "vf", "hf", "cv", // Just looks weird anywhere
                "ht", "hc", "hw",       // Inverted sequences
                "ss", "tt", "rr",
                "[wq][aeiou]$",
                "[aeiou][ghqzpj]$" // weird endings
            );

        }
    }
}
