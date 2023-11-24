using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Syllabore
{
    /// <summary>
    /// The default syllable provider used by a vanilla instance
    /// of <see cref="NameGenerator"/>.
    /// </summary>
    public class DefaultSyllableGenerator : SyllableGenerator
    {
        /// <summary>
        /// Instantiates a new <see cref="DefaultSyllableGenerator"/>
        /// containing a subset of consonants and vowels from the English
        /// language. See further details in the wiki at
        /// <a href="https://github.com/kesac/Syllabore/wiki/DefaultSyllableGenerator">
        /// https://github.com/kesac/Syllabore/wiki/DefaultSyllableGenerator</a>.
        /// </summary>
        public DefaultSyllableGenerator()
        {
            this.WithLeadingConsonants("bdlmprst");
            this.WithLeadingConsonantSequences("st", "ph", "br");
            this.WithVowels("aeio");
            this.WithFinalConsonants("dlmnrstx");
            this.WithFinalConsonantSequences("st", "rn", "lt");
            this.WithProbability(x => x
                    .OfLeadingConsonants(0.95, 0.25)
                    .OfFinalConsonants(0.50, 0.25));
        }
    }
}
