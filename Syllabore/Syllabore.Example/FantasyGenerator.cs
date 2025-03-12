using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    // Outputs fantasy sounding names, maybe
    public class FantasyGenerator : NameGenerator
    {
        public FantasyGenerator()
        {
            this.UsingSyllables(x => x
                    .WithVowels("e").Weight(4)
                    .WithVowels("ai").Weight(2)
                    .WithVowels("uo")
                    .WithLeadingConsonants("lmnstr").Weight(4)
                    .WithLeadingConsonants("kc").Weight(2)
                    .WithLeadingConsonants("yz").Weight(1))
                .UsingTransform(new TransformSet() // Note: only one mutation gets chosen out of these four per call to Next()
                    .RandomlySelect(1)
                    .Add(y => y.AppendSyllable("tia"))
                    .Add(y => y.AppendSyllable("ria"))
                    .Add(y => y.AppendSyllable("lis"))
                    .Add(y => y.AppendSyllable("gar")))
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
