using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// This is the default name filter used in
    /// <see cref="NameGenerator"/>. It eliminates
    /// a few commonly disallowed letter combinations.
    /// </summary>
    public class DefaultNameFilter : NameFilter
    {
        public DefaultNameFilter()
        {
            this.DoNotAllow("[aiuAIU]{2,}"); // Two or more 'a', 'i', or 'u' characters
            this.DoNotAllow("[eoEO]{3,}");   // Three or more 'e' or 'o' characters
            this.DoNotAllow("[^aeiouAEIOU]{3,}"); // Three or more consonant characters
            this.DoNotAllow("[bcdfghjkpqvwxz][^aeiouAEIOU]"); // Consonants that should not have a consonant as the next letter
            this.DoNotAllowEnding("p", "q", "w", "u", "v", "z"); // Awkward sounding endings
        }
    }
}
