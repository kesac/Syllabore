using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example
{
    // Outputs fantasy sounding names, maybe
    public class FantasyGenerator : NameGenerator
    {
        public FantasyGenerator()
        {
            this.UsingProvider(x => x
                    .WithVowels("e").Weight(4)
                    .WithVowels("ai").Weight(2)
                    .WithVowels("uo")
                    .WithLeadingConsonants("lmnstr").Weight(4)
                    .WithLeadingConsonants("kc").Weight(2)
                    .WithLeadingConsonants("yz").Weight(1))
                .UsingTransformer(x => x // Note: only one mutation gets chosen out of these four per call to Next()
                    .WithTransform(y => y.AppendSyllable("tia"))
                    .WithTransform(y => y.AppendSyllable("ria"))
                    .WithTransform(y => y.AppendSyllable("lis"))
                    .WithTransform(y => y.AppendSyllable("gar")))
                .UsingSyllableCount(2, 2);

            // Example output:
            // Terolis
            // Serogar
            // Tecetia
            // Kuregar
            // Meruria

        }
    }
}
