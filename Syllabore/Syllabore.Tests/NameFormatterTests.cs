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
            var sut = new NameFormatter("John {name} Smith")
                    .UsingGenerator("name", new NameGenerator());

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(sut.Next() != String.Empty);
            }
        }

        [TestMethod]
        public void Constructor_WhenMismatchedGeneratorGiven_SuccessfulNameGeneration()
        {
            var sut = new NameFormatter("John {name} Smith")
                    .UsingGenerator("another-property", new NameGenerator());

            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(sut.Next() != String.Empty);
            }
        }

        [TestMethod]
        public void Constructor_WhenPropertyBoundToNull_ExceptionThrown()
        {
            var sut = new NameFormatter("John {name} Smith");
            Assert.ThrowsException<ArgumentNullException>(() => sut.UsingGenerator("name", null));
        }

    }
}
