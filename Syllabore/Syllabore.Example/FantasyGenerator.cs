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
                .UsingMutator(x => x // Note: only one mutation gets chosen out of these four per call to Next()
                    .WithMutation(y => y.AppendSyllable("tia"))
                    .WithMutation(y => y.AppendSyllable("ria"))
                    .WithMutation(y => y.AppendSyllable("lis"))
                    .WithMutation(y => y.AppendSyllable("gar")))
                .LimitSyllableCount(2, 2);

            // Example output:
            // Terolis
            // Serogar
            // Tecetia
            // Kuregar
            // Meruria

        }
    }
}
