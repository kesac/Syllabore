using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorTests
    {
        private readonly NameGenerator _sut = new();

        [TestMethod]
        public void Constructor_WhenNoParameter_SuccessfulNameGeneration()
        {
            for (int i = 0; i < 100; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(_sut.Next()));
            }
        }

        [TestMethod]
        public void Constructor_WhenBasicConstructorUsed_SuccessfulNameGeneration()
        {
            var sut = new NameGenerator("a", "strl");
            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(sut.Next().Contains("a"));
            }
        }


        [TestMethod]
        public void Constructor_WhenAnyParameterNull_ArgumentNullExceptionThrown()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, new TransformSet(), null));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, null, new NameFilter()));
            Assert.ThrowsException<ArgumentNullException>(() => new NameGenerator(null, new TransformSet(), new NameFilter()));

            Assert.IsNotNull(new NameGenerator(new DefaultSyllableGenerator(), null, null).Next());
            Assert.IsNotNull(new NameGenerator(new DefaultSyllableGenerator(), new TransformSet(), null).Next());
            Assert.IsNotNull(new NameGenerator(new DefaultSyllableGenerator(), new TransformSet(), new NameFilter()).Next());
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public void NameGeneration_WhenSyllableLengthPropertyValuesInvalid_SingleArguments_InvalidOperationExceptionThrown(int length)
        {
            Assert.ThrowsException<ArgumentException>(() => _sut.UsingSyllableCount(length).Next());
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(byte.MaxValue)]
        public void NameGeneration_WhenSyllableLengthPropertyValuesValid_SingleArguments_NotNull(int length)
        {
            Assert.IsNotNull(_sut.UsingSyllableCount(length).Next());
        }

        [TestMethod]
        [DataRow(-1, 1)]
        [DataRow(0, 1)]
        [DataRow(1, -1)]
        [DataRow(1, 0)]
        [DataRow(6, 5)]
        public void NameGeneration_WhenSyllableLengthPropertyValuesInvalid_DoubleArguments_InvalidOperationExceptionThrown(int min, int max)
        {
            Assert.ThrowsException<ArgumentException>(() => _sut.UsingSyllableCount(min, max).Next());
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(4, 5)]
        [DataRow(5, 5)]
        public void NameGeneration_WhenSyllableLengthPropertyValuesValid_DoubleArguments_NotNull(int min, int max)
        {
            Assert.IsNotNull(_sut.UsingSyllableCount(min, max).Next());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void NameGeneration_WhenNonPositiveSyllableLengthProvided_ArgumentExceptionThrown(int syllableLength)
        {
            Assert.ThrowsException<ArgumentException>(() => _sut.Next(syllableLength));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(byte.MaxValue)]
        public void NameGeneration_WhenPositiveSyllableLengthProvided_NotNull(int syllableLength)
        {
            Assert.IsNotNull(_sut.Next(syllableLength));
        }

        [TestMethod]
        public void NameGeneration_WhenInfiniteGeneration_ExceptionThrown()
        {
            var sut = new NameGenerator()
                .UsingFilter(x => x
                    .DoNotAllowPattern(".")) // Set filter to reject names with at least 1 character
                    .UsingSyllableCount(10)  // Ensure the generator only produces names with at least 1 character
                    .LimitRetries(1000);  // All futile attempts

            Assert.ThrowsException<InvalidOperationException>(() => sut.Next());

        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        public void NameGeneration_WhenMaximumRetriesLessThanOne_ExceptionThrown(int retryLimit)
        {
            Assert.ThrowsException<ArgumentException>(() => _sut.LimitRetries(retryLimit).Next());
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(byte.MaxValue)]
        public void NameGeneration_WhenMaximumRetriesOneOrMore_NotNull(int retryLimit)
        {
            Assert.IsNotNull(_sut.LimitRetries(retryLimit).Next());
        }

        [TestMethod]
        public void NameGeneration_WithSequencesOnly_Allowed()
        {
            // It is valid for a name generator to use a provider that only uses sequences
            var sut = new NameGenerator()
                .UsingSyllables(x => x
                    .WithLeadingConsonantSequences("sr")
                    .WithVowelSequences("ea")
                    .WithTrailingConsonantSequences("bz")
                    .WithProbability(x => x
                        .OfVowels(1.0)
                        .OfVowelIsSequence(1.0)
                        .OfLeadingVowelsInStartingSyllable(0.0)))
                //.WithProbability(vowelBecomesVowelSequence: 1.0)
                //.DisallowStartingSyllableLeadingVowels()
                //.DisallowLeadingVowelsInStartingSyllables())
                .UsingFilter(x => x.DoNotAllowPattern("^.{,2}$"));// Invalidate names with less than 2 characters

            try
            {
                for (int i = 0; i < 10000; i++)
                {
                    var name = sut.Next();
                    Assert.IsTrue(name.Length > 2);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }


        [TestMethod]
        [DataRow("aeiou", "strlmnp", 0)]
        [DataRow("aeiou", "strlmnp", 12345)]
        public void NameGenerator_StaticRandomSeed_CreatesPredictableOutput(
            string vowels,
            string consonants,
            int seed
        )
        {
            var sut = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels(vowels)
                    .WithConsonants(consonants)
                    .WithRandom(new Random(seed)))
                .UsingRandom(new Random(seed));
            
            var comparison = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels(vowels)
                    .WithConsonants(consonants)
                    .WithRandom(new Random(seed)))
                .UsingRandom(new Random(seed));

            for (int i = 0; i < 1000; i++)
            {
                Assert.AreEqual(sut.Next(), comparison.Next());
            }

        }
    }
}