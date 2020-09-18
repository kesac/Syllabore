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
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(new DefaultSyllableProvider(), null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, new DefaultNameMutator(), null));
            Assert.IsNotNull(new NameGenerator(new DefaultSyllableProvider(), new DefaultNameMutator(), null).Next());
        }

        [TestMethod]
        public void NameGeneration_WhenSyllableLengthPropertyValuesInvalid_InvalidOperationExceptionThrown()
        {
            var generator = new NameGenerator();

            // Single argument
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(-1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(0).Next());
            Assert.IsNotNull(generator.LimitSyllableCount(1).Next());

            // Double argument
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(-1, 1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(0, 1).Next());
            Assert.IsNotNull(generator.LimitSyllableCount(1, 1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(1, -1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(1, 0).Next());
            Assert.IsNotNull(generator.LimitSyllableCount(4, 5).Next());
            Assert.IsNotNull(generator.LimitSyllableCount(5, 5).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitSyllableCount(6, 5).Next());

        }

        [TestMethod]
        public void NameGeneration_WhenNonPositiveSyllableLengthProvided_ArgumentExceptionThrown()
        {
            var generator = new NameGenerator();

            Assert.IsNotNull(generator.Next(1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(0));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(-1));
            Assert.ThrowsException<ArgumentException>(() => generator.Next(int.MinValue));
        }

        [TestMethod]
        public void NameGeneration_WhenInfiniteGeneration_ExceptionThrown()
        {
            var generator = new NameGenerator()
                .UsingValidator(new ConfigurableNameValidator()
                .Invalidate(".")) // Set validator to reject names with at least 1 character
                .LimitSyllableCount(10)  // Ensure the generator only produces names with at least 1 character
                .LimitRetries(1000);  // All futile attempts

            Assert.ThrowsException<InvalidOperationException>(() => generator.Next());

        }

        [TestMethod]
        public void NameGeneration_WhenMaximumRetriesLessThanOne_ExceptionThrown()
        {
            var generator = new NameGenerator();

            Assert.ThrowsException<ArgumentException>(() => generator.LimitRetries(-1).Next());
            Assert.ThrowsException<ArgumentException>(() => generator.LimitRetries(0).Next());
            Assert.IsNotNull(generator.LimitRetries(1).Next());

        }

    }
}
