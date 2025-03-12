using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Syllabore.Tests
{
    [TestClass]
    public class SyllableGeneratorTests
    {
        [TestMethod]
        public void Constructor_NoSymbolGenerators_ThrowsExceptionWhenGenerating()
        {
            var sut = new SyllableGeneratorV3();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
        }

        [TestMethod]
        [DataRow(SymbolPosition.First)]
        [DataRow(SymbolPosition.Middle)]
        [DataRow(SymbolPosition.Last)]
        public void Configuration_AddGeneratorForEachPosition_GeneratesSyllables(SymbolPosition position)
        {
            var symbols = new SymbolGenerator()
                .Add("a").Weight(8)
                .Add("b").Weight(4)
                .Cluster("c");

            var sut = new SyllableGeneratorV3()
                .Add(position, symbols);

            var counts = new int[3];
            for (int i = 0; i < 1000; i++)
            {
                var syllable = sut.Next();
                if (syllable.Contains("a")) { counts[0]++; }
                else if (syllable.Contains("b")) { counts[1]++; }
                else if (syllable.Contains("c")) { counts[2]++; }
            }

            Assert.IsTrue(counts[0] > counts[1]);
            Assert.IsTrue(counts[1] > counts[2]);
            Assert.IsTrue(counts[2] > 0);

        }

        [TestMethod]
        public void Configuration_AddGeneratorAllPositions_GeneratesFullSyllables()
        {
            var sut = new SyllableGeneratorV3();
            sut.Add(SymbolPosition.First, new SymbolGenerator().Add("a"));
            sut.Add(SymbolPosition.Middle, new SymbolGenerator().Add("b"));
            sut.Add(SymbolPosition.Last, new SymbolGenerator().Add("c"));
            Assert.IsTrue(sut.Next().Equals("abc"));
        }

        [TestMethod]
        public void Configuration_AddGeneratorUsingShortform_GeneratesFullSyllables()
        {
            var sut = new SyllableGeneratorV3();
            sut.Add(SymbolPosition.First, "a");
            sut.Add(SymbolPosition.Middle, "b");
            sut.Add(SymbolPosition.Last, "c");
            Assert.IsTrue(sut.Next().Equals("abc"));
        }

        [TestMethod]
        [DataRow(1000)]
        public void Configuration_ChangePositionProbability_ProbabilityAffectsGeneration(int attempts)
        {
            var sut = new SyllableGeneratorV3()
                .Add(SymbolPosition.First, new SymbolGenerator().Add("a"))
                .Add(SymbolPosition.Middle, new SymbolGenerator().Add("b"))
                .Add(SymbolPosition.Last, new SymbolGenerator().Add("c"))
                .Chance(SymbolPosition.First, 0.5)
                .Chance(SymbolPosition.Last, 0.5);

            // The first and last symbols don't always appear
            // That makes four theoretical combinations
            var possibleOutput = new[] { "abc", "ab", "bc", "b" };
            var outputDetected = new Dictionary<string, bool>();

            for (int i = 0; i < attempts; i++)
            {
                outputDetected[sut.Next()] = true;
            }

            // Check that all possible combinations are generated
            foreach (var target in possibleOutput)
            {
                Assert.IsTrue(outputDetected.ContainsKey(target));
            }

            // Check if any extra combination was generated
            Assert.IsTrue(outputDetected.Count == possibleOutput.Length);

        }

        [TestMethod]
        [DataRow("aeiou", "strlmnp", 0)]
        [DataRow("aeiou", "strlmnp", 12345)]
        public void SyllableGenerator_StaticRandomSeed_CreatesPredictableOutput(
            string consonants,
            string vowels,
            int seed
        )
        {
            var consonantSymbols = new SymbolGenerator() { Random = new Random(seed) };
            consonantSymbols.Add(consonants);

            var consonantSymbols2 = consonantSymbols.Copy();
            consonantSymbols2.Random = new Random(seed);

            var vowelSymbols = new SymbolGenerator() { Random = new Random(seed) };
            vowelSymbols.Add(vowels);

            var vowelSymbols2 = vowelSymbols.Copy();
            vowelSymbols2.Random = new Random(seed);

            var sut = new SyllableGeneratorV3() { Random = new Random(seed) };
            sut.Add(SymbolPosition.First, consonantSymbols).Add(SymbolPosition.Middle, vowelSymbols);

            var comparison = new SyllableGeneratorV3() { Random = new Random(seed) };
            comparison.Add(SymbolPosition.First, consonantSymbols2).Add(SymbolPosition.Middle, vowelSymbols2);

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(sut.Next(), comparison.Next());
            }

        }

    }
}