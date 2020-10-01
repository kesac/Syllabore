using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class SyllableProviderTests
    {
        [TestMethod]
        public void Provider_WhenCustomVowelsDefined_OnlyCustomVowelsAppearInProviderOutput()
        {
            // Define one and only one vowel then check name output
            var provider = new SyllableProvider()
                    .WithVowels("a")
                    .WithLeadingConsonants("bcdfg")
                    .WithTrailingConsonants("bcdfg");

            for (int i = 0; i < 1000; i++)
            {
                var startingSyllable = provider.NextStartingSyllable();
                var syllable = provider.NextSyllable();
                var endingSyllable = provider.NextEndingSyllable();
                Assert.IsTrue(startingSyllable.Contains("a"));
                Assert.IsTrue(syllable.Contains("a"));
                Assert.IsTrue(endingSyllable.Contains("a"));
                Assert.IsFalse(Regex.IsMatch(startingSyllable, "[e|i|o|u]"));
                Assert.IsFalse(Regex.IsMatch(syllable, "[e|i|o|u]"));
                Assert.IsFalse(Regex.IsMatch(endingSyllable, "[e|i|o|u]"));
            }

            // Change the one and only vowel and check name output again
            provider = new SyllableProvider()
                .WithVowels("y")
                .WithLeadingConsonants("bcdfg")
                .WithTrailingConsonants("bcdfg");

            for (int i = 0; i < 1000; i++)
            {
                var startingSyllable = provider.NextStartingSyllable();
                var syllable = provider.NextSyllable();
                var endingSyllable = provider.NextEndingSyllable();
                Assert.IsTrue(startingSyllable.Contains("y"));
                Assert.IsTrue(syllable.Contains("y"));
                Assert.IsTrue(endingSyllable.Contains("y"));
                Assert.IsFalse(Regex.IsMatch(startingSyllable, "[a|e|i|o|u]"));
                Assert.IsFalse(Regex.IsMatch(syllable, "[a|e|i|o|u]"));
                Assert.IsFalse(Regex.IsMatch(endingSyllable, "[a|e|i|o|u]"));
            }

        }

        [TestMethod]
        public void Provider_WhenCustomVowelsDefined_OnlyCustomVowelsAppearInNameGeneratorOutput()
        {
            // Define one and only one vowel then check name output
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("a")
                    .WithLeadingConsonants("bcdfg")
                    .WithTrailingConsonants("bcdfg"));

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Contains("a"));
                Assert.IsFalse(Regex.IsMatch(name, "[e|i|o|u]"));
            }

            // Change the one and only vowel and check name output again
            generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("y")
                    .WithLeadingConsonants("bcdfg")
                    .WithTrailingConsonants("bcdfg"));

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Contains("y"));
                Assert.IsFalse(Regex.IsMatch(name, "[a|e|i|o|u]"));
            }

        }

        [TestMethod]
        public void Provider_WithOnlyVowelsDefined_SyllablesGeneratedCorrectly()
        {
            // Define all vowels
            var provider = new SyllableProvider().WithVowels("aeiou");

            // Check that every syllable-generating method works and generates nothing but the vowels
            for (int i = 0; i < 1000; i++)
            {
                var startingSyllable = provider.NextStartingSyllable();
                var syllable = provider.NextStartingSyllable();
                var endingSyllable = provider.NextEndingSyllable();

                Assert.IsTrue(!string.IsNullOrEmpty(startingSyllable) && Regex.IsMatch(startingSyllable, "^[aeiouAEIOU]+$"));
                Assert.IsTrue(!string.IsNullOrEmpty(syllable) && Regex.IsMatch(syllable, "^[aeiouAEIOU]+$"));
                Assert.IsTrue(!string.IsNullOrEmpty(endingSyllable) && Regex.IsMatch(endingSyllable, "^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void Provider_WithOnlyVowelsDefined_NameGenerationWorks()
        {
            // Define all vowels
            var generator = new NameGenerator().UsingProvider(x => x.WithVowels("aeiou"));

            for (int i = 0; i < 1000; i++)
            {
                // Check that the name generator generates nothing but the vowels
                var name = generator.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name) && Regex.IsMatch(name, "^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void Provider_WithNoVowelsDefined_ProviderThrowsInvalidOperationException()
        {
            // Define everything except vowel and vowel sequences
            var provider = new SyllableProvider()
                .WithLeadingConsonants("b")
                .WithLeadingConsonantSequences("bb", "bbb")
                .WithTrailingConsonants("b")
                .WithTrailingConsonantSequences("bbbb", "bbbbb");

            // Syllables always require a vowel so syllable generation should fail
            for (int i = 0; i < 1000; i++)
            {
                Assert.ThrowsException<InvalidOperationException>(() => provider.NextStartingSyllable());
                Assert.ThrowsException<InvalidOperationException>(() => provider.NextSyllable());
                Assert.ThrowsException<InvalidOperationException>(() => provider.NextEndingSyllable());
            }
        }

        [TestMethod]
        public void Provider_WithNoVowelsDefined_NameGeneratorThrowsInvalidOperationException()
        {
            // Same as Provider_WithNoVowelsDefined_ProviderThrowsInvalidOperationException
            // except it applies to a NameGenerator

            var provider = new SyllableProvider()
                .WithLeadingConsonants("b")
                .WithLeadingConsonantSequences("bb", "bbb")
                .WithTrailingConsonants("b")
                .WithTrailingConsonantSequences("bbbb", "bbbbb");

            var generator = new NameGenerator().UsingProvider(provider);
                    
            for (int i = 0; i < 1000; i++)
            {
                Assert.ThrowsException<InvalidOperationException>(() => generator.Next());
            }

        }

        [TestMethod]
        public void Provider_WithCustomVowelProbability_AffectsNameGenerationSuccess()
        {
            // Defining at least one vowel sequence, but no individual vowels also shouldn't work if
            // the probability of vowel sequences is not set to 100%
            var provider = new SyllableProvider().WithVowelSequences("aa");
            var generator = new NameGenerator().UsingProvider(provider);

            Assert.ThrowsException<InvalidOperationException>(() => // Some will succeed, but expecting some to fail
            {
                for (int i = 0; i < 1000; i++)
                {
                    generator.Next();
                }
            });

            // Defining at least one vowel sequence, set to 100% probability
            // without any possibility of vowels starting name shoudl work
            // provider.WithVowelSequenceProbability(1.0).WithStartingSyllableLeadingVowelSequenceProbability(1.0);
            provider.WithProbability(x => x.OfVowelSequences(1.0).OfStartingSyllableLeadingVowelSequence(1.0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsNotNull(generator.Next());
            }

            // The normal scenario where at least one individual vowel is defined
            provider.WithVowels("a").WithProbability(x => x.OfVowelSequences(0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsNotNull(generator.Next());
            }

        }

        public void Provider_WithCustomComponents_AllComponentsAppearInProviderOutput()
        {
            // In this test we define one instance of a vowel, vowel sequence
            // leading consonant, leading consonant sequence, trailing consonant
            // and trailing consonant sequence, then check that each instance occurs
            // at least once in the provider's syllable generation.

            var provider = new SyllableProvider()
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff")
                    .WithProbability(y => y
                        .OfVowelSequences(0.5)
                        .OfStartingSyllableLeadingVowels(0.25)
                        .OfLeadingConsonantSequences(0.50)
                        .OfTrailingConsonants(0.50));

            bool foundVowel = false;
            bool foundVowelSequence = false;
            bool foundStartingConsonant = false;
            bool foundStartingConsonantSequence = false;
            bool foundEndingConsonant = false;
            bool foundEndingConsonantSequence = false;

            // Starting syllables only
            for (int i = 0; i < 500; i++)
            {
                var syllable = provider.NextStartingSyllable();
                if (syllable.Contains("a")) { foundVowel = true; }
                if (syllable.Contains("ee")) { foundVowelSequence = true; }
                if (syllable.Contains("b")) { foundStartingConsonant = true; }
                if (syllable.Contains("cc")) { foundStartingConsonantSequence = true; }
                if (syllable.Contains("d")) { foundEndingConsonant = true; }
                if (syllable.Contains("ff")) { foundEndingConsonantSequence = true; }
            }

            Assert.IsTrue(foundVowel);
            Assert.IsTrue(foundVowelSequence);
            Assert.IsTrue(foundStartingConsonant);
            Assert.IsTrue(foundStartingConsonantSequence);
            Assert.IsTrue(foundEndingConsonant);
            Assert.IsTrue(foundEndingConsonantSequence);

            // Reset
            foundVowel = false;
            foundVowelSequence = false;
            foundStartingConsonant = false;
            foundStartingConsonantSequence = false;
            foundEndingConsonant = false;
            foundEndingConsonantSequence = false;

            // All syllables
            for (int i = 0; i < 500; i++)
            {
                var syllable = provider.NextSyllable();
                if (syllable.Contains("a")) { foundVowel = true; }
                if (syllable.Contains("ee")) { foundVowelSequence = true; }
                if (syllable.Contains("b")) { foundStartingConsonant = true; }
                if (syllable.Contains("cc")) { foundStartingConsonantSequence = true; }
                if (syllable.Contains("d")) { foundEndingConsonant = true; }
                if (syllable.Contains("ff")) { foundEndingConsonantSequence = true; }
            }

            Assert.IsTrue(foundVowel);
            Assert.IsTrue(foundVowelSequence);
            Assert.IsTrue(foundStartingConsonant);
            Assert.IsTrue(foundStartingConsonantSequence);
            Assert.IsTrue(foundEndingConsonant);
            Assert.IsTrue(foundEndingConsonantSequence);

            // Reset
            foundVowel = false;
            foundVowelSequence = false;
            foundStartingConsonant = false;
            foundStartingConsonantSequence = false;
            foundEndingConsonant = false;
            foundEndingConsonantSequence = false;

            // Ending syllables only
            for (int i = 0; i < 500; i++)
            {
                var syllable = provider.NextEndingSyllable();
                if (syllable.Contains("a")) { foundVowel = true; }
                if (syllable.Contains("ee")) { foundVowelSequence = true; }
                if (syllable.Contains("b")) { foundStartingConsonant = true; }
                if (syllable.Contains("cc")) { foundStartingConsonantSequence = true; }
                if (syllable.Contains("d")) { foundEndingConsonant = true; }
                if (syllable.Contains("ff")) { foundEndingConsonantSequence = true; }
            }

            Assert.IsTrue(foundVowel);
            Assert.IsTrue(foundVowelSequence);
            Assert.IsTrue(foundStartingConsonant);
            Assert.IsTrue(foundStartingConsonantSequence);
            Assert.IsTrue(foundEndingConsonant);
            Assert.IsTrue(foundEndingConsonantSequence);

        }

        [TestMethod]
        public void Provider_WithCustomComponents_AllComponentsAppearInNameGeneratorOutput()
        {
            // Same as Provider_WithCustomComponents_AllComponentsAppearInProviderOutput
            // but checking output of a NameGenerator

            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff")
                    .WithProbability(y => y
                        .OfVowelSequences(0.5)
                        .OfStartingSyllableLeadingVowels(0.25)
                        .OfLeadingConsonantSequences(0.50)
                        .OfTrailingConsonants(0.50)));

            bool foundVowel = false;
            bool foundVowelSequence = false;
            bool foundStartingConsonant = false;
            bool foundStartingConsonantSequence = false;
            bool foundEndingConsonant = false;
            bool foundEndingConsonantSequence = false;

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                if (name.Contains("a")) { foundVowel = true; }
                if (name.Contains("ee")) { foundVowelSequence = true; }
                if (name.Contains("b")) { foundStartingConsonant = true; }
                if (name.Contains("cc")) { foundStartingConsonantSequence = true; }
                if (name.Contains("d")) { foundEndingConsonant = true; }
                if (name.Contains("ff")) { foundEndingConsonantSequence = true; }
            }

            Assert.IsTrue(foundVowel);
            Assert.IsTrue(foundVowelSequence);
            Assert.IsTrue(foundStartingConsonant);
            Assert.IsTrue(foundStartingConsonantSequence);
            Assert.IsTrue(foundEndingConsonant);
            Assert.IsTrue(foundEndingConsonantSequence);

        }

        [TestMethod, Timeout(10000)]
        public void Provider_WithNoSequencesConfigured_NameGeneratorStillGeneratesNames()
        {
            // It is valid for a name generator to use a provider with no sequences defined
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonants("srt")
                    .WithVowels("ea")
                    .WithTrailingConsonants("tz"))
                .UsingValidator(x => x.DoNotAllowPattern("^.{,2}$"));// Invalidate names with less than 3 characters

            try
            {
                for (int i = 0; i < 10000; i++)
                {
                    var name = generator.Next();
                    Assert.IsTrue(name.Length > 2);
                }
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        [TestMethod]
        public void Provider_WithSequencesConfiguredAtOneHundredPercent_NameGeneratorIsOk()
        {
            // It is valid for a name generator to use a provider that only uses sequences
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonantSequences("sr")
                    .WithVowelSequences("ea")
                    .WithTrailingConsonantSequences("bz")
                    .WithProbability(x => x.OfVowelSequences(1.0))
                    .DisallowStartingSyllableLeadingVowels())
                .UsingValidator(x => x.DoNotAllowPattern("^.{,2}$"));// Invalidate names with less than 3 characters

            try
            {
                for (int i = 0; i < 10000; i++)
                {
                    var name = generator.Next();
                    Assert.IsTrue(name.Length > 2);
                }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

        }

        // This is just a helper method to provide a vanilla
        // provider with one instance of every vowel/consonant 
        // combination defined
        private static SyllableProvider GetDefaultProviderWithAllComponentsDefined()
        {
            return new SyllableProvider()
                .WithLeadingConsonants("b")
                .WithLeadingConsonantSequences("cc")
                .WithTrailingConsonants("d")
                .WithTrailingConsonantSequences("ff")
                .WithVowels("o")
                .WithVowelSequences("uu");
        }

        [TestMethod]
        public void Provider_WithTogglingOfStartingSyllableLeadingVowels_TurnsOnOrOffAsExpected()
        {
            // By default, vowels have a probability of starting a name

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool leadingVowelDetected = false; 
            for(int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o","u")) { leadingVowelDetected = true; break; }
            }
            Assert.IsTrue(leadingVowelDetected);

            provider.DisallowStartingSyllableLeadingVowels(); // You can turn this off explicitly without adjusting probability settings
            leadingVowelDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o", "u")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);

            provider.AllowStartingSyllableLeadingVowels(); // You can also turn this on explicitly
            leadingVowelDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o", "u")) { leadingVowelDetected = true; break; }
            }
            Assert.IsTrue(leadingVowelDetected);
        }

        [TestMethod]
        public void Provider_WithTogglingOfLeadingConsonants_TurnsOnOrOffAsExpected()
        {
            // By default, consonants can start a syllable

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool leadingConsonantDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantDetected);

            provider.DisallowLeadingConsonants(); // You can turn this off explicitly without adjusting probability settings, this should also affect sequences
            leadingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsFalse(leadingConsonantDetected);

            provider.AllowLeadingConsonants(); // You can also turn this on explicitly
            leadingConsonantDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantDetected);
        }

        [TestMethod]
        public void Provider_WithTogglingOfLeadingConsonantSequences_TurnsOnOrOffAsExpected()
        {
            // By default, consonant sequences can start a syllable

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool leadingConsonantSequenceDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { leadingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantSequenceDetected);

            provider.DisallowLeadingConsonantSequences(); // You can turn this off explicitly without adjusting probability settings
            leadingConsonantSequenceDetected = false; // In which case we expect this variable to remain false
            bool leadingConsonantDetected = false; // This should be true as the flag only affects sequences
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { leadingConsonantSequenceDetected = true; }
                else if (s.StartsWith("b")) { leadingConsonantDetected = true; }
            }
            Assert.IsFalse(leadingConsonantSequenceDetected);
            Assert.IsTrue(leadingConsonantDetected);

            provider.AllowLeadingConsonantSequences(); // You can also turn this on explicitly
            leadingConsonantSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { leadingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantSequenceDetected);
        }

        [TestMethod]
        public void Provider_WithTogglingOfVowelSequences_TurnsOnOrOffAsExpected()
        {
            // By default, consonant sequences can start a syllable

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool vowelSequenceDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsTrue(vowelSequenceDetected);

            provider.DisallowVowelSequences(); // You can turn this off explicitly without adjusting probability settings
            vowelSequenceDetected = false; // In which case we expect this variable to remain false
            
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsFalse(vowelSequenceDetected);

            provider.AllowVowelSequences(); // You can also turn this on explicitly
            vowelSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsTrue(vowelSequenceDetected);
        }

        [TestMethod]
        public void Provider_WithTogglingOfTrailingConsonants_TurnsOnOrOffAsExpected()
        {
            // By default, consonants can start a syllable

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool trailingConsonantDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantDetected);

            provider.DisallowTrailingConsonants(); // You can turn this off explicitly without adjusting probability settings, this should also affect sequences
            trailingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsFalse(trailingConsonantDetected);

            provider.AllowTrailingConsonants(); // You can also turn this on explicitly
            trailingConsonantDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantDetected);
        }

        [TestMethod]
        public void Provider_WithTogglingOfLeadingConsonantSequenceSequences_TurnsOnOrOffAsExpected()
        {
            // By default, consonant sequences can start a syllable

            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool trailingConsonantSequenceDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("ff")) { trailingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantSequenceDetected);

            provider.DisallowTrailingConsonantSequences(); // You can turn this off explicitly without adjusting probability settings
            trailingConsonantSequenceDetected = false; // In which case we expect this variable to remain false
            bool trailingConsonantDetected = false; // This should be true as the flag only affects sequences
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("ff")) { trailingConsonantSequenceDetected = true; }
                else if (s.EndsWith("d")) { trailingConsonantDetected = true; }
            }
            Assert.IsFalse(trailingConsonantSequenceDetected);
            Assert.IsTrue(trailingConsonantDetected);

            provider.AllowTrailingConsonantSequences(); // You can also turn this on explicitly
            trailingConsonantSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { trailingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantSequenceDetected);
        }

    }
}
