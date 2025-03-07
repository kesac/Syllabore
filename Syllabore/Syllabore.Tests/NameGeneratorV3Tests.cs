using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorV3Tests
    {
        [TestMethod]
        public void Constructor_NoSyllableGenerators_ThrowsExceptionWhenGenerating()
        {
            var sut = new NameGeneratorV3();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
        }

        [TestMethod]
        public void Configuration_AddGeneratorForFirstPosition_GeneratesName()
        {
            var symbols = new SymbolGenerator().Add("a");
            var syllables = new SyllableGeneratorV3().Add(Position.First, symbols);

            var sut = new NameGeneratorV3()
                .Add(Position.First, syllables)
                .Size(1);

            var name = sut.Next();
            Assert.IsTrue(name.Equals("A"));
        }

        [TestMethod]
        public void Configuration_AddGeneratorsForFirstAndLastPositions_GeneratesName()
        {
            var firstSymbols = new SymbolGenerator().Add("c");
            var lastSymbols = new SymbolGenerator().Add("d");

            var syllables = new SyllableGeneratorV3()
                .Add(Position.First, firstSymbols)
                .Add(Position.Last, lastSymbols);

            var sut = new NameGeneratorV3()
                .Add(Position.First, syllables)
                .Add(Position.Last, syllables)
                .Size(1, 2);

            HashSet<string> detected = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                detected.Add(name);
            }

            Assert.IsTrue(detected.Contains("Cd"));
            Assert.IsTrue(detected.Contains("Cdcd"));
        }

        [TestMethod]
        public void Configuration_AddGeneratorsForAllPositions_GeneratesName()
        {
            var firstSymbols = new SymbolGenerator().Add("t");
            var middleSymbols = new SymbolGenerator().Add("u");
            var lastSymbols = new SymbolGenerator().Add("k");

            var syllables = new SyllableGeneratorV3()
                .Add(Position.First, firstSymbols)
                .Add(Position.Middle, middleSymbols)
                .Add(Position.Last, lastSymbols);

            var sut = new NameGeneratorV3()
                .Add(Position.First, syllables)
                .Add(Position.Middle, syllables)
                .Add(Position.Last, syllables)
                .Size(1, 3);

            HashSet<string> detected = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                detected.Add(name);
            }

            Assert.IsTrue(detected.Contains("Tuk"));
            Assert.IsTrue(detected.Contains("Tuktuk"));
            Assert.IsTrue(detected.Contains("Tuktuktuk"));
        }

        [DataTestMethod]
        [DataRow(1, 2)]
        [DataRow(2, 4)]
        [DataRow(3, 10)]
        public void Configuration_SetMinimumAndMaximumSize_GeneratesNameWithinSizeRange(int min, int max)
        {
            var firstSymbols = new SymbolGenerator().Add("a");
            var middleSymbols = new SymbolGenerator().Add("b");
            var lastSymbols = new SymbolGenerator().Add("c");

            var sut = new NameGeneratorV3()
                .Add(Position.First, new SyllableGeneratorV3().Add(Position.First, firstSymbols))
                .Add(Position.Middle, new SyllableGeneratorV3().Add(Position.Middle, middleSymbols))
                .Add(Position.Last, new SyllableGeneratorV3().Add(Position.Last, lastSymbols))
                .Size(min, max);

            // Test name generation multiple times to ensure correctness
            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.Length >= min && name.Length <= max);
            }
        }

        [DataTestMethod]
        [DataRow("a", "b")]
        [DataRow("ab", "cd")]
        [DataRow("abc", "def")]
        public void Constructor_TwoStringParameters_GeneratesName(string firstSymbols, string middleSymbols)
        {
            var sut = new NameGeneratorV3(firstSymbols, middleSymbols).Size(1);

            // Build a list of expected combinations
            var expected = new List<string>();
            foreach (char f in firstSymbols)
            {
                foreach (char m in middleSymbols)
                {
                    string combo = char.ToUpper(f).ToString() + m;
                    expected.Add(combo);
                }
            }

            // Generate 100 names
            var detected = new Dictionary<string, bool>();
            for (int i = 0; i < 100; i++)
            {
                string name = sut.Next();
                detected[name] = true;
            }

            // Verify that every expected combination appeared at least once.
            foreach (var e in expected)
            {
                Assert.IsTrue(detected.ContainsKey(e), "Not every combination detected");
            }

            Assert.IsTrue(detected.Count <= expected.Count, "More combinations detected than was expected");
        }

        [TestMethod]
        public void Constructor_ThreeStringParameters_GeneratesName()
        {
            var sut = new NameGeneratorV3("a", "b", "c").Size(1);
            var name = sut.Next();
            Assert.AreEqual("Abc", name);
        }
    }
}
