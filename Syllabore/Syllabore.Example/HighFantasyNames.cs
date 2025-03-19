using System;
using System.Collections.Generic;
using System.Linq;
using Archigen;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    /// <summary>
    /// High-fantasy sounding names
    /// </summary>
    public class HighFantasyNames : Example
    {
        // Builds a NameGenerator that generates names with
        // three syllables like: Terolis, Kuregar, Serogar, Tecetia, Meruria
        public IGenerator<string> GetGenerator()
        {
            var names = new NameGenerator()
                .All(x => x
                .First(x => x
                    .Add("lmnstr").Weight(4)
                    .Add("kc").Weight(2)
                    .Add("yz"))
                .Middle(x => x
                    .Add("e").Weight(4)
                    .Add("ai").Weight(2)
                    .Add("uo")));

            names.SetSize(2);

            names.Transform(new TransformSet()
                .RandomlySelect(1)
                .Add(x => x.Append("tia"))
                .Add(x => x.Append("ria"))
                .Add(x => x.Append("lis"))
                .Add(x => x.Append("gar")));

            // Calling Next() on this class will result in a name
            // that has 3 syllables (2 + 1 from the transform set)

            return names;
        }

        // Builds the same NameGenerator as GetGenerator(),
        // but without using Syllabore.Fluent extension methods
        public NameGenerator GetNonFluentGenerator()
        {
            var consonants = new SymbolGenerator()
                .Add("lmnstr").Weight(4)
                .Add("kc").Weight(2)
                .Add("yz");

            var vowels = new SymbolGenerator()
                .Add("e").Weight(4)
                .Add("ai").Weight(2)
                .Add("uo");

            var syllables = new SyllableGenerator()
                .Add(SymbolPosition.First, consonants)
                .Add(SymbolPosition.Middle, vowels);

            var suffixes = new string[] { "tia", "ria", "lis", "gar" };
            var suffixTransform = new TransformSet().RandomlySelect(1);

            foreach (var suffix in suffixes)
            {
                suffixTransform.Add(new Transform().Append(suffix));
            }

            var names = new NameGenerator();
            names.SetSyllables(SyllablePosition.Any, syllables);
            names.SetTransform(suffixTransform);
            names.SetSize(2);

            return names;
        }
    }
}
