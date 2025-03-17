using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class SymbolGeneratorTests
    {
        [TestMethod]
        public void Constructor_NoSymbols_ThrowsExceptionWhenGenerating()
        {
            var sut = new SymbolGenerator();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
        }

        [TestMethod]
        [DataRow("a")]
        [DataRow("Ƣ")]
        [DataRow("🙂")]
        public void SymbolGenerator_AddOneSymbol_CanGenerateSymbol(string symbol)
        {
            var sut = new SymbolGenerator();
            sut.Add(symbol);

            var result = sut.Next();
            Assert.AreEqual(result, symbol);
        }

        [TestMethod]
        [DataRow("ae")]
        [DataRow("Ƣʯ🜧")]
        [DataRow("🙂🙂")]
        public void SymbolGenerator_AddOneCluster_CanGenerateCluster(string cluster)
        {
            var sut = new SymbolGenerator();
            sut.Cluster(cluster);

            var result = sut.Next();
            Assert.AreEqual(result, cluster);
        }

        [TestMethod]
        public void Constructor_AddMultipleSymbols_CanGenerateSymbols()
        {
            var sut = new SymbolGenerator();

            sut.Add("a");
            Assert.IsTrue(sut.Symbols.Count == 1);

            sut.Add("Ƣcd").Add("🜧f");
            Assert.IsTrue(sut.Symbols.Count == 6);

            sut.Cluster("gh", "🙂🙂").Cluster("kl");
            Assert.IsTrue(sut.Symbols.Count == 9);

            for (int i = 0; i < 100; i++)
            {
                var symbol = sut.Next();
                Assert.IsTrue(symbol.Length >= 1);
            }

        }

        [TestMethod]
        public void Constructor_AddWeightedSymbols_HigherWeightsAffectGeneration()
        {
            var sut = new SymbolGenerator();

            sut.Add("a").Weight(8); // We expect this to be generated the most
            sut.Cluster("bc").Weight(4);
            sut.Add("d").Weight(1); // We expect this to be generated the least

            // Generate 1000 symbols and count how many of each appear
            var counts = new int[3];
            for (int i = 0; i < 1000; i++)
            {
                var symbol = sut.Next();

                if(symbol == "a") { counts[0]++; }
                else if (symbol == "bc") { counts[1]++; }
                else if (symbol == "d") { counts[2]++; }
            }

            Assert.IsTrue(counts[0] > counts[1]);
            Assert.IsTrue(counts[1] > counts[2]);
            Assert.IsTrue(counts[2] > 0);

        }

    }
}