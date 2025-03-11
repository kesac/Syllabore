using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Syllabore.Fluent;
using System.Globalization;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorV3FluentTests
    {
        private SyllableGeneratorV3 GetSyllableGenerator(string first, string middle, string last)
        {
            return new SyllableGeneratorV3()
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
            var sut = new NameGeneratorV3()
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
                .Size(3);

            // Then build a reference name generator using non-fluent methods

            var referenceNameGenerator = new NameGeneratorV3()
                .Set(SyllablePosition.Leading, GetSyllableGenerator(symbols[0], symbols[1], symbols[2]))
                .Set(SyllablePosition.Inner, GetSyllableGenerator(symbols[3], symbols[4], symbols[5]))
                .Set(SyllablePosition.Trailing, GetSyllableGenerator(symbols[6], symbols[7], symbols[8]))
                .Size(3);

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
            var sut = new NameGeneratorV3()
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
                .Size(3);


            // Then build a reference name generator using non-fluent methods
            var referenceNameGenerator = new NameGeneratorV3()
                .Set(SyllablePosition.Leading, GetSyllableGenerator(symbols[0], symbols[1], symbols[2]))
                .Set(SyllablePosition.Inner, GetSyllableGenerator(symbols[3], symbols[4], symbols[5]))
                .Set(SyllablePosition.Trailing, GetSyllableGenerator(symbols[6], symbols[7], symbols[8]))
                .Size(3);

            // And compare the output of both generators
            var reference = referenceNameGenerator.Next();
            var result = sut.Next();

            Assert.IsTrue(result == targetName);
            Assert.IsTrue(result == reference);
        }

        [TestMethod]
        public void Fluent_UseCopyMethods_GeneratesNames()
        {
            var sut = new NameGeneratorV3()
                .Lead(x => x
                    .First("a")
                    .Middle("b")
                    .Last("c"))
                .Inner(x => x.CopyLead())
                .Trail(x => x.CopyInner())
                .Lead(x => x.CopyTrail())
                .Size(3);

            var abcSymbols = GetSyllableGenerator("a", "b", "c");

            var referenceNameGenerator = new NameGeneratorV3()
                .Set(SyllablePosition.Leading, abcSymbols)
                .Set(SyllablePosition.Inner, abcSymbols)
                .Set(SyllablePosition.Trailing, abcSymbols)
                .Size(3);

            var reference = referenceNameGenerator.Next();
            var result = sut.Next();

            Assert.IsTrue(result == "Abcabcabc");
            Assert.IsTrue(result == reference);
        }

    }

}
