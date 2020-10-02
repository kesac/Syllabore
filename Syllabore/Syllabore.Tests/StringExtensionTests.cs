using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void StringExtension_ReplaceLeadingSyllable()
        {
            var name = new Name("hel", "lo");
            Assert.IsTrue(name.ToString() == "Hello");

            name.ReplaceLeadingSyllable("hi");
            Assert.IsTrue(name.ToString() == "Hilo");

        }

        [TestMethod]
        public void StringExtension_StartsWithVowel()
        {
            var name = new Name("hel", "lo");
            Assert.IsFalse(name.StartsWithVowel());

            name.ReplaceLeadingSyllable("el");
            Assert.IsTrue(name.StartsWithVowel());
        }

    }
}
