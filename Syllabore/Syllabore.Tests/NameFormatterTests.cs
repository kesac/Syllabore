using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameFormatterTests
    {

        [TestMethod]
        public void Constructor_WithNoParameter_ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NameFormatter(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("Test")]
        public void Constructor_WithUnformattedParameter_SuccessfulNameGeneration(string format)
        {
            var sut = new NameFormatter(format);
            Assert.IsTrue(sut.Next() == format);
        }

        [TestMethod]
        public void Constructor_WithFormattedParameter_SuccessfulNameGeneration()
        {
            var sut = new NameFormatter("John {name} Smith {number}")
                    .Define("name", new NameGenerator("str", "ae"))
                    .Define("number", new NameGenerator("123","456"));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next();
                Assert.IsTrue(result != String.Empty);
                Assert.IsFalse(result.Contains("{name}"));
                Assert.IsFalse(result.Contains("{number}"));
            }
        }

        [TestMethod]
        public void Constructor_WhenMismatchedGeneratorGiven_SuccessfulNameGeneration()
        {
            var sut = new NameFormatter("John {name} Smith")
                    .Define("another-property", new NameGenerator("str", "ae"));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next();
                Assert.IsTrue(result != String.Empty);
                Assert.IsTrue(result.Contains("{name}"));
            }
        }

        [TestMethod]
        public void Constructor_WhenPropertyBoundToNull_ExceptionThrown()
        {
            var sut = new NameFormatter("John {name} Smith");
            Assert.ThrowsException<ArgumentNullException>(() => sut.Define("name", null));
        }

    }
}
