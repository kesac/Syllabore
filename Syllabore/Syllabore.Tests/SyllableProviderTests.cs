using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class SyllableProviderTests
    {


        // This is just a helper method to provide a vanilla
        // provider with one instance of every vowel/consonant 
        // combination defined
        private static SyllableProvider GetDefaultProviderWithAllComponentsDefined()
        {
            return new SyllableProvider()
                .WithLeadingConsonants("b").Sequences("cc")
                .WithTrailingConsonants("d").Sequences("ff")
                .WithVowels("o").Sequences("uu")
                .WithProbability(x => x.LeadingConsonantExists(1.0));
        }

        private bool EachOutputNeverContainsAnyOf(SyllableProvider p, params string[] invalidSubstrings)
        {
            bool outputNeverAppears = true;
            for (int i = 0; i < 1000; i++)
            {
                var s = p.NextSyllable();
                if (s.ContainsAny(invalidSubstrings))
                {
                    outputNeverAppears = false;
                    break;
                }
            }

            return outputNeverAppears;
        }

        private bool EachOutputContainsAnyOf(SyllableProvider p, params string[] validSubstrings)
        {
            bool outputAlwaysAppears = true;
            for (int i = 0; i < 1000; i++)
            {
                var s = p.NextSyllable();
                if (!s.ContainsAny(validSubstrings))
                {
                    outputAlwaysAppears = false;
                    break;
                }
            }

            return outputAlwaysAppears;
        }

        private bool AllOutputContainsAtLeastOnce(SyllableProvider p, params string[] validSubstrings)
        {
            bool[] substringAppeared = new bool[validSubstrings.Length];

            for (int i = 0; i < 1000; i++)
            {
                var s = p.NextSyllable();

                for (int j = 0; j < validSubstrings.Length; j++)
                {
                    if (s.Contains(validSubstrings[j]))
                    {
                        substringAppeared[j] = true;
                    }
                }
            }

            return substringAppeared.All(x => x);

        }

        [TestMethod]
        public void Provider_WithNothingDefined_ThrowsException()
        {
            // Instantiating a provider, but not defining any vowels, consonants, probabilities
            // will produce empty strings, which by default will cause an error to be thrown
            var provider = new SyllableProvider();
            Assert.ThrowsException<InvalidOperationException>(() => provider.NextStartingSyllable());
            Assert.ThrowsException<InvalidOperationException>(() => provider.NextSyllable());
            Assert.ThrowsException<InvalidOperationException>(() => provider.NextEndingSyllable());
        }

        [TestMethod]
        public void Provider_WithNothingDefinedButEmptyStringsEnabled_Allowed()
        {
            // Instantiating a provider, but not defining any vowels, consonants, probabilities
            // will produce empty strings, which by default will cause an error to be thrown
            var provider = new SyllableProvider().AllowEmptyStrings(true);
            Assert.AreEqual(provider.NextStartingSyllable(), string.Empty);
            Assert.AreEqual(provider.NextSyllable(), string.Empty);
            Assert.AreEqual(provider.NextEndingSyllable(), string.Empty);
        }

        [TestMethod]
        public void Provider_WithOneVowelsDefinedThroughProvider_OnlyTheOneVowelAppearsInOutput()
        {
            // Define one and only one vowel then check name output
            var provider = new SyllableProvider()
                    .WithVowels("a")
                    .WithConsonants("bcdfg");

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
                .WithConsonants("bcdfg");

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
        public void Provider_WithOnlyVowelsDefined_Allowed()
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
        public void Provider_WithNoVowelsDefined_AllowedIfConfiguredCorrectly()
        {
            // If only consonants are defined, but not configured to appear 100% of the time,
            // syllable generation should throw an error because empty strings are not permitted
            // by providers by default 

            var provider = new SyllableProvider()
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("bb", "bbb")
                    .WithTrailingConsonants("b")
                    .WithTrailingConsonantSequences("bbbb", "bbbbb")
                    .WithProbability(x => x
                        .LeadingConsonantExists(0.5)
                        .LeadingConsonantBecomesSequence(0.5)
                        .TrailingConsonantExists(0.5)
                        .TrailingConsonantBecomesSequence(0.5));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    provider.NextStartingSyllable();
                    provider.NextSyllable();
                    provider.NextEndingSyllable();
                }
            });

            // If only consonants are defined, but consonant probability is 100%,
            // syllable generation will work normally and output will never be an empty string.

            provider.WithProbability(x => x
                    .LeadingConsonantExists(1)
                    .LeadingConsonantBecomesSequence(0.5)
                    .TrailingConsonantExists(1)
                    .TrailingConsonantBecomesSequence(0.5));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(provider.NextStartingSyllable().Length > 0);
                Assert.IsTrue(provider.NextSyllable().Length > 0);
                Assert.IsTrue(provider.NextEndingSyllable().Length > 0);
            }
        }


        [TestMethod]
        public void Provider_WithCustomVowelProbability_AffectsNameGenerationSuccess()
        {
            // Defining at least one vowel sequence, but no individual vowels also shouldn't work if
            // the probability of vowel sequences is not set to 100%
            var provider = new SyllableProvider()
                .WithVowelSequences("aa")
                .WithProbability(x => x
                    .VowelExists(1.0)
                    .VowelBecomesSequence(0));

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
            provider.WithProbability(x => x
                .VowelExists(1.0)
                .VowelBecomesSequence(1.0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(generator.Next()));
            }
            
            // The normal scenario where at least one individual vowel is defined
            provider.WithVowels("a")
                    .WithProbability(x => x.VowelBecomesSequence(0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(generator.Next()));
            }
            
        }

        [TestMethod]
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
                    .WithProbability(x => x
                        .VowelBecomesSequence(0.50)
                        .LeadingConsonantExists(0.50)
                        .TrailingConsonantExists(0.50)
                        .StartingSyllable.LeadingVowelExists(0.25));

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
                    .WithVowels("a").Sequences("ee")
                    .WithLeadingConsonants("b").Sequences("cc")
                    .WithTrailingConsonants("d").Sequences("ff")
                    .WithProbability(x => x
                        .VowelBecomesSequence(0.50)
                        .LeadingConsonantExists(0.50)
                        .TrailingConsonantExists(0.50)));

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
                    .WithTrailingConsonants("tz")
                    .WithProbability(x => x.LeadingConsonantExists(1.0)))
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
        public void Provider_SettingLeadingVowelInStartingSyllable_AffectsOutput()
        {
            // 1. By default, vowels do not have a probability of starting a name
            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool leadingVowelDetected = false; 
            for(int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);

            // 2. Leading vowels in the starting syllables can be enabled
            // by increasing the probability to non-zero
            provider.WithProbability(x => x.StartingSyllable.LeadingVowelExists(0.5)); // You can turn this off explicitly without adjusting probability settings
            leadingVowelDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o")) { leadingVowelDetected = true; break; }
            }
            Assert.IsTrue(leadingVowelDetected);

            // 2. Leading vowels in the starting syllables can be disabled
            // by setting the probability to zero
            provider.WithProbability(x => x.StartingSyllable.LeadingVowelExists(0));
            leadingVowelDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);
        }

        [TestMethod]
        public void Provider_SettingLeadingVowelSequenceInStartingSyllable_AffectsOutput()
        {
            // 1. By default, vowel sequences do not have a probability of starting a name
            var provider = GetDefaultProviderWithAllComponentsDefined();
            bool leadingVowelDetected = false;
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("uu")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);

            // 2. Leading vowel sequences in the starting syllables can be enabled
            // by increasing the probability to non-zero
            provider.WithProbability(x => x
                .StartingSyllable.LeadingVowelExists(0.5)
                .StartingSyllable.LeadingVowelBecomesSequence(0.5)); // You can turn this off explicitly without adjusting probability settings
            leadingVowelDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("uu")) { leadingVowelDetected = true; break; }
            }
            Assert.IsTrue(leadingVowelDetected);

            // 2. Leading vowel sequences in the starting syllables can be disabled
            // by setting the probability to zero
            provider.WithProbability(x => x
                .StartingSyllable.LeadingVowelExists(0.5)
                .StartingSyllable.LeadingVowelBecomesSequence(0.0)); 
            leadingVowelDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("uu")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);
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

            // provider.WithLeadingConsonants().Chance(0); 
            provider.WithProbability(x => x.LeadingConsonantExists(0)); // This also prevents sequences

            leadingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsFalse(leadingConsonantDetected);

            // provider.AllowLeadingConsonants(); // You can also turn this on explicitly
            // provider.WithLeadingConsonants().Chance(0.50);
            provider.WithProbability(x => x.LeadingConsonantExists(0.5));
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

            // provider.DisallowLeadingConsonantSequences();
            // provider.WithLeadingConsonantSequences().Chance(0);
            provider.WithProbability(x => x.LeadingConsonantBecomesSequence(0));
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

            // provider.LetLeadingConsonantSequences(0.5);
            // provider.WithLeadingConsonantSequences().Chance(0.50);
            provider.WithProbability(x => x.LeadingConsonantBecomesSequence(0.5));
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

            // provider.WithVowelSequences().Chance(0); // You can turn this off explicitly without adjusting probability settings
            provider.WithProbability(x => x.VowelBecomesSequence(0));
            vowelSequenceDetected = false; // In which case we expect this variable to remain false
            
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsFalse(vowelSequenceDetected);

            // provider.WithVowelSequences().Chance(0.50); // You can also turn this on explicitly
            provider.WithProbability(x => x.VowelBecomesSequence(0.5));
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

            // provider.WithTrailingConsonants().Chance(0.0); // You can turn this off explicitly without adjusting probability settings, this should also affect sequences
            provider.WithProbability(x => x.TrailingConsonantExists(0));
            trailingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsFalse(trailingConsonantDetected);

            //provider.WithTrailingConsonants().Chance(0.50); // You can also turn this on explicitly
            provider.WithProbability(x => x.TrailingConsonantExists(0.5));
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

            // provider.WithTrailingConsonantSequences().Chance(0); // You can turn this off explicitly without adjusting probability settings
            provider.WithProbability(x => x.TrailingConsonantBecomesSequence(0));
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

            // provider.WithTrailingConsonantSequences().Chance(0.5); // You can also turn this on explicitly
            provider.WithProbability(x => x.TrailingConsonantBecomesSequence(0.5));
            trailingConsonantSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { trailingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantSequenceDetected);
        }


        [TestMethod]
        public void Provider_CustomConsonantProbabilityDefined_AffectsSyllableGenerationCorrectly()
        {

            var provider = new SyllableProvider()
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff");

            // Disable all leading and trailing consonants 
            Assert.IsTrue(EachOutputNeverContainsAnyOf(
                provider.WithProbability(x => x
                            .LeadingConsonantExists(0)
                            .LeadingConsonantBecomesSequence(0)
                            .TrailingConsonantExists(0)
                            .TrailingConsonantBecomesSequence(0)),
                    "b", "cc", "d", "ff"));

            // Consonant sequence probability doesn't matter
            // if the consonant probability is zero
            Assert.IsTrue(EachOutputNeverContainsAnyOf(
                provider.WithProbability(x => x
                            .LeadingConsonantExists(0)
                            .LeadingConsonantBecomesSequence(1)
                            .TrailingConsonantExists(0)
                            .TrailingConsonantBecomesSequence(1)),
                    "b", "cc", "d", "ff"));

            // Consonant sequence probability only matters
            // if the consonant probability is not zero

            provider.WithProbability(x => x
                            .LeadingConsonantExists(1)
                            .LeadingConsonantBecomesSequence(0)
                            .TrailingConsonantExists(1)
                            .TrailingConsonantBecomesSequence(0));

            // There should be no consonant sequences
            Assert.IsTrue(EachOutputContainsAnyOf(provider, "b", "d"));
            Assert.IsTrue(EachOutputNeverContainsAnyOf(provider, "cc", "ff"));

            provider.WithProbability(x => x
                            .LeadingConsonantExists(1)
                            .LeadingConsonantBecomesSequence(1)
                            .TrailingConsonantExists(1)
                            .TrailingConsonantBecomesSequence(1));

            // There should always be consonant sequences
            Assert.IsTrue(EachOutputNeverContainsAnyOf(provider, "b", "d"));
            Assert.IsTrue(EachOutputContainsAnyOf(provider, "cc", "ff"));

            // Test whether a value between 0 and 1 ouputs both consonants and consonant sequences
            provider.WithProbability(x => x
                            .LeadingConsonantExists(1)
                            .LeadingConsonantBecomesSequence(0.5)
                            .TrailingConsonantExists(1)
                            .TrailingConsonantBecomesSequence(0.5));

            Assert.IsTrue(AllOutputContainsAtLeastOnce(provider, "b", "d", "cc", "ff"));

            // Not all output will have a consonant
            provider.WithProbability(x => x
                            .LeadingConsonantExists(0.5)
                            .LeadingConsonantBecomesSequence(0.5)
                            .TrailingConsonantExists(0.5)
                            .TrailingConsonantBecomesSequence(0.5));

            Assert.IsTrue(AllOutputContainsAtLeastOnce(provider, "b", "d", "cc", "ff"));

        }


    }
}
