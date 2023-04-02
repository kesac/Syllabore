using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameFormatterTests
    {

        [TestMethod]
        public void Constructor_WithNoParameter_SuccessfulNameGeneration()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NameFormatter(null));
        }

        [TestMethod]
        public void Constructor_WithParameter_SuccessfulNameGeneration()
        {

            Assert.IsTrue(new NameFormatter(string.Empty).Next() == string.Empty);
            Assert.IsTrue(new NameFormatter("Test").Next() == "Test");

            var f = new NameFormatter("John {name} Smith")
                    .UsingGenerator("name", new NameGenerator());

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(f.Next() != String.Empty);
            }

        }

    }
}
