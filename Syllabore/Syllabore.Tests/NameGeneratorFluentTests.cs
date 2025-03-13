using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Syllabore.Fluent;
using System.Globalization;
using System.Collections.Generic;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorFluentTests
    {
        private SyllableGenerator GetSyllableGenerator(string first, string middle, string last)
        {
            return new SyllableGenerator()
                .Add(SymbolPosition.First, first)
                .Add(SymbolPosition.Middle, middle)
                .Add(SymbolPosition.Last, last);
        }

        [TestMethod]
        [DataRow("Abcdefghi")]
        public void Fluent_ConfigureBasicNameGenerator_GeneratesNames(string targetName)
        {
            if (new StringInfo(targetName).LengthInTextElements != 9)
            {
                throw new ArgumentException("In this test, the target name must be 9 characters long.");
            }

            string[] symbols = targetName.Atomize().ToArray();

            // Build a name generator using fluent methods that accept strings
            var sut = new NameGenerator()
                .Lead(x => x
                    .First(symbols[0])
                    .Middle(symbols[1])
                    .Last(symbols[2]))
                .Inner(x => x
                    .First(symbols[3])
                    .Middle(symbols[4])
                    .Last(symbols[5]))
                .Trail(x => x
                    .First(symbols[6])
                    .Middle(symbols[7])
                    .Last(symbols[8]))
                .SetSize(3);

            // Then build a reference name generator using non-fluent methods

            var referenceNameGenerator = new NameGenerator()
                .SetSyllables(SyllablePosition.Leading, GetSyllableGenerator(symbols[0], symbols[1], symbols[2]))
                .SetSyllables(SyllablePosition.Inner, GetSyllableGenerator(symbols[3], symbols[4], symbols[5]))
                .SetSyllables(SyllablePosition.Trailing, GetSyllableGenerator(symbols[6], symbols[7], symbols[8]))
                .SetSize(3);

            // And compare the output of both generators
            var reference = referenceNameGenerator.Next();
            var result = sut.Next();

            Assert.IsTrue(result == targetName);
            Assert.IsTrue(result == reference);
        }

        [TestMethod]
        [DataRow("Abcdefghi")]
        public void Fluent_ProvideSymbolGenerators_GeneratesNames(string targetName)
        {
            if (targetName.Length != 9)
            {
                throw new ArgumentException("In this test, the target name must be 9 characters long.");
            }

            string[] symbols = targetName.Atomize().ToArray();

            // Build a name generator using fluent methods
            // that accept SymbolGenerators
            var sut = new NameGenerator()
                .Lead(x => x
                    .First(x => x.Add(symbols[0]))
                    .Middle(x => x.Add(symbols[1]))
                    .Last(x => x.Add(symbols[2])))
                .Inner(x => x
                    .First(x => x.Add(symbols[3]))
                    .Middle(x => x.Add(symbols[4]))
                    .Last(x => x.Add(symbols[5])))
                .Trail(x => x
                    .First(x => x.Add(symbols[6]))
                    .Middle(x => x.Add(symbols[7]))
                    .Last(x => x.Add(symbols[8])))
                .SetSize(3);


            // Then build a reference name generator using non-fluent methods
            var referenceNameGenerator = new NameGenerator()
                .SetSyllables(SyllablePosition.Leading, GetSyllableGenerator(symbols[0], symbols[1], symbols[2]))
                .SetSyllables(SyllablePosition.Inner, GetSyllableGenerator(symbols[3], symbols[4], symbols[5]))
                .SetSyllables(SyllablePosition.Trailing, GetSyllableGenerator(symbols[6], symbols[7], symbols[8]))
                .SetSize(3);

            // And compare the output of both generators
            var reference = referenceNameGenerator.Next();
            var result = sut.Next();

            Assert.IsTrue(result == targetName);
            Assert.IsTrue(result == reference);
        }

        [TestMethod]
        public void Fluent_UseCopyMethods_GeneratesNames()
        {
            var sut = new NameGenerator()
                .Lead(x => x
                    .First("a")
                    .Middle("b")
                    .Last("c"))
                .Inner(x => x.CopyLead())
                .Trail(x => x.CopyInner())
                .Lead(x => x.CopyTrail())
                .SetSize(3);

            var abcSymbols = GetSyllableGenerator("a", "b", "c");

            var referenceNameGenerator = new NameGenerator()
                .SetSyllables(SyllablePosition.Leading, abcSymbols)
                .SetSyllables(SyllablePosition.Inner, abcSymbols)
                .SetSyllables(SyllablePosition.Trailing, abcSymbols)
                .SetSize(3);

            var reference = referenceNameGenerator.Next();
            var result = sut.Next();

            Assert.IsTrue(result == "Abcabcabc");
            Assert.IsTrue(result == reference);
        }

        [TestMethod]
        public void Fluent_UseWeightMethod_WeightAffectSymbols()
        {
            var sut = new NameGenerator()
                .Lead(x => x
                    .First("a").Chance(0.5)
                    .Middle("b")
                    .Last("c"))
                .Inner(x => x.CopyLead())
                .Trail(x => x.CopyInner())
                .SetSize(3);

            var results = new HashSet<string>();

            for(int i = 0; i < 100; i++)
            {
                results.Add(sut.Next());
            }

            Assert.IsTrue(results.Contains("Abcabcabc"));
            Assert.IsTrue(results.Contains("Bcbcbc"));
        }

    }

}
