using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            var firstSymbols = new SymbolGenerator().Add("a");
            var lastSymbols = new SymbolGenerator().Add("b");
            var sut = new NameGeneratorV3()
                .Add(Position.First, new SyllableGeneratorV3().Add(Position.First, firstSymbols))
                .Add(Position.Last, new SyllableGeneratorV3().Add(Position.Last, lastSymbols));

            var name = sut.Next();
            Assert.IsTrue(name.Equals("Ab"));
        }

        [TestMethod]
        public void Configuration_AddGeneratorsForAllPositions_GeneratesName()
        {
            var firstSymbols = new SymbolGenerator().Add("a");
            var middleSymbols = new SymbolGenerator().Add("b");
            var lastSymbols = new SymbolGenerator().Add("c");
            var sut = new NameGeneratorV3()
                .Add(Position.First, new SyllableGeneratorV3().Add(Position.First, firstSymbols))
                .Add(Position.Middle, new SyllableGeneratorV3().Add(Position.Middle, middleSymbols))
                .Add(Position.Last, new SyllableGeneratorV3().Add(Position.Last, lastSymbols))
                .Size(3);

            var name = sut.Next();
            Assert.IsTrue(name.Equals("Abc"));
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
    }
}
