using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Syllabore.Fluent;
using System.Collections.Generic;
using System.Text;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorTests
    {
        [TestMethod]
        public void Constructor_NoSyllableGenerators_ThrowsExceptionWhenGenerating()
        {
            var sut = new NameGenerator();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
        }

        [TestMethod]
        [DataRow(SymbolPosition.First)]
        [DataRow(SymbolPosition.Middle)]
        [DataRow(SymbolPosition.Last)]
        public void Configuration_AddGeneratorForFirstPosition_GeneratesName(SymbolPosition symbolPosition)
        {
            var symbols = new SymbolGenerator().Add("a");
            var syllables = new SyllableGenerator().Add(symbolPosition, symbols);

            var sut = new NameGenerator()
                .SetSyllables(SyllablePosition.Any, syllables)
                .SetSize(1);

            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.Equals("A"));
            }
        }

        [TestMethod]
        public void Configuration_AddGeneratorsForFirstAndLastPositions_GeneratesName()
        {
            var firstSymbols = new SymbolGenerator().Add("c");
            var lastSymbols = new SymbolGenerator().Add("d");

            var syllables = new SyllableGenerator()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Last, lastSymbols);

            var sut = new NameGenerator()
                .SetSyllables(SyllablePosition.Starting, syllables)
                .SetSyllables(SyllablePosition.Ending, syllables)
                .SetSize(1, 2);

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

            var syllables = new SyllableGenerator()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Middle, middleSymbols)
                .Add(SymbolPosition.Last, lastSymbols);

            var sut = new NameGenerator()
                .SetSyllables(SyllablePosition.Starting, syllables)
                .SetSyllables(SyllablePosition.Inner, syllables)
                .SetSyllables(SyllablePosition.Ending, syllables)
                .SetSize(1, 3);

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

            var sut = new NameGenerator()
                .SetSyllables(SyllablePosition.Starting, new SyllableGenerator().Add(SymbolPosition.First, firstSymbols))
                .SetSyllables(SyllablePosition.Inner, new SyllableGenerator().Add(SymbolPosition.Middle, middleSymbols))
                .SetSyllables(SyllablePosition.Ending, new SyllableGenerator().Add(SymbolPosition.Last, lastSymbols))
                .SetSize(min, max);

            // Test name generation multiple times to ensure correctness
            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.Length >= min && name.Length <= max);
            }
        }

        [DataTestMethod]
        [DataRow("a", "b")]
        [DataRow("À", "Ƣ")]
        [DataRow("🙂", "🂅ヅ")]
        [DataRow("ab", "cd")]
        [DataRow("abc", "def")]
        [DataRow("Àßç", "Ƣʯ🜧")]
        public void Constructor_TwoStringParameters_GeneratesName(string firstSymbols, string middleSymbols)
        {
            var sut = new NameGenerator(firstSymbols, middleSymbols).SetSize(1);

            // Build a list of expected combinations
            var expected = new List<string>();
            foreach (string f in firstSymbols.Atomize())
            {
                foreach (string m in middleSymbols.Atomize())
                {
                    var combo = new StringBuilder();
                    combo.Append(f.ToUpper());
                    combo.Append(m.ToLower());
                    
                    expected.Add(combo.ToString());
                }
            }

            // Generate 100 names
            var detected = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                string name = sut.Next();
                detected.Add(name);
            }

            // Verify that every expected combination appeared at least once.
            foreach (var e in expected)
            {
                Assert.IsTrue(detected.Contains(e), "Not every combination detected: could not find '" + e + "'");
            }

            Assert.IsTrue(detected.Count <= expected.Count, "More combinations detected than was expected");
        }

        [TestMethod]
        public void Constructor_ThreeStringParameters_GeneratesName()
        {
            var sut = new NameGenerator("a", "b", "c").SetSize(1);
            var name = sut.Next();
            Assert.AreEqual("Abc", name);
        }
    }
}