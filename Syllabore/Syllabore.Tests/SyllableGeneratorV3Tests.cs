using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Syllabore.Tests
{
    [TestClass]
    public class SyllableGeneratorV3Tests
    {
        [TestMethod]
        public void Constructor_NoSymbolGenerators_ThrowsExceptionWhenGenerating()
        {
            var sut = new SyllableGeneratorV3();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
        }

        [TestMethod]
        [DataRow(Position.First)]
        [DataRow(Position.Middle)]
        [DataRow(Position.Last)]
        public void Configuration_AddGeneratorForEachPosition_GeneratesSyllables(Position position)
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
            sut.Add(Position.First, new SymbolGenerator().Add("a"));
            sut.Add(Position.Middle, new SymbolGenerator().Add("b"));
            sut.Add(Position.Last, new SymbolGenerator().Add("c"));
            Assert.IsTrue(sut.Next().Equals("abc"));
        }

        [TestMethod]
        public void Configuration_AddGeneratorUsingShortform_GeneratesFullSyllables()
        {
            var sut = new SyllableGeneratorV3();
            sut.Add(Position.First, "a");
            sut.Add(Position.Middle, "b");
            sut.Add(Position.Last, "c");
            Assert.IsTrue(sut.Next().Equals("abc"));
        }

        [TestMethod]
        [DataRow(1000)]
        public void Configuration_ChangePositionProbability_ProbabilityAffectsGeneration(int attempts)
        {
            var sut = new SyllableGeneratorV3()
                .Add(Position.First, new SymbolGenerator().Add("a"))
                .Add(Position.Middle, new SymbolGenerator().Add("b"))
                .Add(Position.Last, new SymbolGenerator().Add("c"))
                .Chance(Position.First, 0.5)
                .Chance(Position.Last, 0.5);

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

    }
}