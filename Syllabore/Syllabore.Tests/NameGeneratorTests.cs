using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorTests
    {
        [TestMethod]
        public void Constructor_WhenAnyParameterNull_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(new StandaloneSyllableProvider(), null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, new StandaloneNameValidator()));
        }

        [TestMethod]
        public void NameGeneration_WhenSyllableLengthPropertyValuesInvalid_InvalidOperationExceptionThrown()
        {
            var generator = new NameGenerator(new StandaloneSyllableProvider());

            generator.MinimumSyllables = -1;
            generator.MaximumSyllables = 1;
            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

            generator.MinimumSyllables = 0;
            generator.MaximumSyllables = 1;
            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

            generator.MinimumSyllables = 1;
            generator.MaximumSyllables = 1;
            Assert.IsNotNull(generator.Next());

            generator.MinimumSyllables = 1;
            generator.MaximumSyllables = -1;
            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

            generator.MinimumSyllables = 1;
            generator.MaximumSyllables = 0;
            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

            generator.MinimumSyllables = 4;
            generator.MaximumSyllables = 5;
            Assert.IsNotNull(generator.Next());

            generator.MinimumSyllables = 5;
            generator.MaximumSyllables = 5;
            Assert.IsNotNull(generator.Next());

            generator.MinimumSyllables = 6;
            generator.MaximumSyllables = 5;
            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

        }

        [TestMethod]
        public void NameGeneration_WhenNonPositiveSyllableLengthProvided_ArgumentExceptionThrown()
        {
            var generator = new NameGenerator(new StandaloneSyllableProvider());
            Assert.IsNotNull(generator.Next(1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(0));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(-1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(int.MinValue));
        }
    }
}
