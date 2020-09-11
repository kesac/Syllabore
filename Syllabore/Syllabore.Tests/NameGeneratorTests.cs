using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorTests
    {
        [TestMethod]
        public void Constructor_WhenNoParameter_SuccessfulNameGeneration()
        {
            var generator = new NameGenerator();
            for(int i = 0; i < 100; i++)
            {
                Assert.IsTrue(generator.Next() != string.Empty);
            }
        }

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
            var generator = new NameGenerator();

            // Single argument
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(-1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(0).Next());
            Assert.IsNotNull(generator.SetSyllableCount(1).Next());

            // Double argument
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(-1, 1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(0, 1).Next());
            Assert.IsNotNull(generator.SetSyllableCount(1, 1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(1, -1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(1, 0).Next());
            Assert.IsNotNull(generator.SetSyllableCount(4, 5).Next());
            Assert.IsNotNull(generator.SetSyllableCount(5, 5).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetSyllableCount(6, 5).Next());

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

        [TestMethod]
        public void NameGeneration_WhenInfiniteGeneration_ExceptionThrown()
        {
            var generator = new NameGenerator()
                .SetValidator(new ConfigurableNameValidator()
                .AddRegexConstraint(".")) // Set validator to reject names with at least 1 character
                .SetSyllableCount(10)  // Ensure the generator only produces names with at least 1 character
                .SetMaximumRetries(10000);  // All futile attempts

            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

        }

        [TestMethod]
        public void NameGeneration_WhenMaximumRetriesLessThanOne_ExceptionThrown()
        {
            var generator = new NameGenerator();

            Assert.ThrowsException<ArgumentException>(() => generator.SetMaximumRetries(-1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.SetMaximumRetries(0).Next());
            Assert.IsNotNull(generator.SetMaximumRetries(1).Next());

        }

    }
}
