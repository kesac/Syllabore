using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorTests
    {
        [TestMethod]
        public void Constructor_WhenAnyParameterNull_ExceptionThrown()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(new StandaloneSyllableProvider(), null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, new StandaloneNameValidator()));
        }

        [TestMethod]
        public void NameGeneration_WhenNonPositiveSyllableLengthSpecified_ExceptionThrown()
        {
            var generator = new NameGenerator(new StandaloneSyllableProvider());
            Assert.IsNotNull(generator.Next(1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(0));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(-1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(int.MinValue));
        }
    }
}
