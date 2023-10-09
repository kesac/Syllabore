using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// The default syllable provider used by a vanilla instance
    /// of <see cref="NameGenerator"/>.
    /// </para>
    /// </summary>
    public class DefaultSyllableGenerator : SyllableGenerator
    {
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
