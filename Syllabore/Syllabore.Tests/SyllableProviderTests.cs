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
        private static SyllableGenerator GetDefaultProviderWithAllComponentsDefined()
        {
            return new SyllableGenerator()
                .WithLeadingConsonants("b").Sequences("cc")
                .WithTrailingConsonants("d").Sequences("ff")
                .WithVowels("o").Sequences("uu")
                .WithProbability(x => x.OfLeadingConsonants(1.0));
        }

        // This is just a helper method to provide a vanilla
        // provider with one instance of every vowel/consonant 
        // combination defined
        private static SyllableGenerator GetConsonantOnlyGenerator()
        {
            return new SyllableGenerator()
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("bb", "bbb")
                    .WithTrailingConsonants("b")
                    .WithTrailingConsonantSequences("bbbb", "bbbbb");
        }

        /// <summary>
        /// Helper method to check that the output of a provider never contains 
        /// <strong>any</strong> of the specified substrings.
        /// </summary>
        private bool EachOutputNeverContainsAnyOf(SyllableGenerator p, params string[] invalidSubstrings)
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

        /// <summary>
        /// Helper method to check that the output of a provider always contains at least one
        /// instance of <strong>any</strong> of the specified substrings.
        /// </summary>
        private bool EachOutputContainsAnyOf(SyllableGenerator p, params string[] validSubstrings)
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

        /// <summary>
        /// Helper method to check that the output of a provider always contains at least one
        /// instance of <strong>all</strong> of the specified substrings.
        /// </summary>
        private bool AllOutputContainsAtLeastOnce(SyllableGenerator p, params string[] validSubstrings)
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
        public void SyllableGenerator_NoGraphemes_StartingSyllableGenerationThrowsException()
        {
            // Instantiating a provider, but not defining any vowels, consonants, probabilities
            // will produce empty strings, which by default will cause an error to be thrown
            var sut = new SyllableGenerator();
            Assert.ThrowsException<InvalidOperationException>(() => sut.NextStartingSyllable());
        }

        [TestMethod]
        public void SyllableGenerator_NoGraphemes_NextSyllableGenerationThrowsException()
        {
            var sut = new SyllableGenerator();
            Assert.ThrowsException<InvalidOperationException>(() => sut.NextSyllable());
        }

        [TestMethod]
        public void SyllableGenerator_NoGraphemes_EndingSyllableGenerationThrowsException()
        {
            var sut = new SyllableGenerator();
            Assert.ThrowsException<InvalidOperationException>(() => sut.NextEndingSyllable());
        }

        [TestMethod]
        public void SyllableGenerator_NoGraphemesWithEmptyStringAllowed_StartingSyllableGenerationAllowed()
        {
            var sut = new SyllableGenerator().AllowEmptyStrings(true);
            Assert.AreEqual(sut.NextStartingSyllable(), string.Empty);
        }

        [TestMethod]
        public void SyllableGenerator_NoGraphemesWithEmptyStringAllowed_NextSyllableGenerationAllowed()
        {
            var sut = new SyllableGenerator().AllowEmptyStrings(true);
            Assert.AreEqual(sut.NextSyllable(), string.Empty);
        }

        [TestMethod]
        public void SyllableGenerator_NoGraphemesWithEmptyStringAllowed_EndingSyllableGenerationAllowed()
        {
            var sut = new SyllableGenerator().AllowEmptyStrings(true);
            Assert.AreEqual(sut.NextEndingSyllable(), string.Empty);
        }

        [TestMethod]
        public void SyllableGenerator_WithOneVowel_VowelAppearsInOutput()
        {
            // Define one and only one vowel then check name output
            var sut = new SyllableGenerator("a", "bcdfg");

            for (int i = 0; i < 1000; i++)
            {
                var startingSyllable = sut.NextStartingSyllable();
                var syllable = sut.NextSyllable();
                var endingSyllable = sut.NextEndingSyllable();

                Assert.IsTrue(startingSyllable.Contains("a"));
                Assert.IsTrue(syllable.Contains("a"));
                Assert.IsTrue(endingSyllable.Contains("a"));
            }
        }


        [TestMethod]
        public void SyllableGenerator_OnlyVowelsWithNoConsonants_GenerationSuccessful()
        {
            // Define all vowels
            var sut = new SyllableGenerator().WithVowels("aeiou");

            // Check that every syllable-generating method works and generates nothing but the vowels
            for (int i = 0; i < 1000; i++)
            {
                var startingSyllable = sut.NextStartingSyllable();
                var syllable = sut.NextStartingSyllable();
                var endingSyllable = sut.NextEndingSyllable();

                Assert.IsTrue(Regex.IsMatch(startingSyllable, "^[aeiouAEIOU]+$"));
                Assert.IsTrue(Regex.IsMatch(syllable, "^[aeiouAEIOU]+$"));
                Assert.IsTrue(Regex.IsMatch(endingSyllable, "^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void NameGenerator_OnlyVowelsWithNoConsonants_GenerationSuccessful()
        {
            // Define all vowels
            var sut = new NameGenerator().UsingSyllables(x => x.WithVowels("aeiou"));

            for (int i = 0; i < 1000; i++)
            {
                // Check that the name generator generates nothing but the vowels
                var name = sut.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name) && Regex.IsMatch(name, "^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithoutGuaranteedConsonants1_NotAllowed()
        {
            // If only consonants are defined, but not configured to appear 100% of the time,
            // syllable generation should throw an error because empty strings are not permitted
            // by providers by default 
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(0.5, 0.5)
                    .OfTrailingConsonants(0.5, 0.5));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                        sut.NextStartingSyllable();
                }
            });
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithoutGuaranteedConsonants2_NotAllowed()
        {
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(0.5, 0.5)
                    .OfTrailingConsonants(0.5, 0.5));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                        sut.NextSyllable();
                }
            });
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithoutGuaranteedConsonants3_NotAllowed()
        {
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(0.5, 0.5)
                    .OfTrailingConsonants(0.5, 0.5));

            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                for (int i = 0; i < 1000; i++)
                {
                        sut.NextEndingSyllable();
                }
            });
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithGuaranteedConsonants1_Allowed()
        {
            // If only consonants are defined, but configured to appear 100% of the time,
            // syllable generation should still work
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0, 0.5)
                    .OfTrailingConsonants(1.0, 0.5));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(sut.NextStartingSyllable().Length > 0);
            }
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithGuaranteedConsonants2_Allowed()
        {
            // If only consonants are defined, but configured to appear 100% of the time,
            // syllable generation should still work
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0, 0.5)
                    .OfTrailingConsonants(1.0, 0.5));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(sut.NextSyllable().Length > 0);
            }
        }

        [TestMethod]
        public void SyllableGenerator_NoVowelsDefinedWithGuaranteedConsonants3_Allowed()
        {
            // If only consonants are defined, but configured to appear 100% of the time,
            // syllable generation should still work
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0, 0.5)
                    .OfTrailingConsonants(1.0, 0.5));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(sut.NextEndingSyllable().Length > 0);
            }
        }


        // TODO

        [TestMethod]
        public void SyllableGenerator_WithCustomVowelProbability_AffectsNameGenerationSuccess()
        {
            // Defining at least one vowel sequence, but no individual vowels also shouldn't work if
            // the probability of vowel sequences is not set to 100%
            var provider = new SyllableGenerator()
                .WithVowelSequences("aa")
                .WithProbability(x => x
                    .OfVowels(1.0, 0));

            var generator = new NameGenerator().UsingSyllables(provider);

            
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
                .OfVowels(1.0, 1.0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(generator.Next()));
            }
            
            // The normal scenario where at least one individual vowel is defined
            provider.WithVowels("a")
                    .WithProbability(x => x.OfVowelIsSequence(0));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(generator.Next()));
            }
            
        }

        [TestMethod]
        public void SyllableGenerator_WithCustomComponents_AllComponentsAppearInProviderOutput()
        {
            // In this test we define one instance of a vowel, vowel sequence
            // leading consonant, leading consonant sequence, trailing consonant
            // and trailing consonant sequence, then check that each instance occurs
            // at least once in the provider's syllable generation.

            var provider = new SyllableGenerator()
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff")
                    .WithProbability(x => x
                        .OfVowels(1, 0.50)
                        .OfLeadingConsonants(0.50)
                        .OfTrailingConsonants(0.50)
                        .OfLeadingVowelsInStartingSyllable(0.25));

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
        public void SyllableGenerator_WithCustomComponents_AllComponentsAppearInNameGeneratorOutput()
        {
            // Same as SyllableGenerator_WithCustomComponents_AllComponentsAppearInProviderOutput
            // but checking output of a NameGenerator

            var generator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithVowels("a").Sequences("ee")
                    .WithLeadingConsonants("b").Sequences("cc")
                    .WithTrailingConsonants("d").Sequences("ff")
                    .WithProbability(x => x
                        .OfVowelIsSequence(0.50)
                        .OfLeadingConsonants(0.50)
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
        public void SyllableGenerator_WithNoSequencesConfigured_NameGeneratorStillGeneratesNames()
        {
            // It is valid for a name generator to use a provider with no sequences defined
            var generator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithLeadingConsonants("srt")
                    .WithVowels("ea")
                    .WithTrailingConsonants("tz")
                    .WithProbability(x => x.OfLeadingConsonants(1.0)))
                .UsingFilter(x => x.DoNotAllowPattern("^.{,2}$"));// Invalidate names with less than 3 characters

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
        public void SyllableGenerator_SettingLeadingVowelInStartingSyllable_AffectsOutput()
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
            provider.WithProbability(x => x.OfLeadingVowelsInStartingSyllable(0.5)); // You can turn this off explicitly without adjusting probability settings
            leadingVowelDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o")) { leadingVowelDetected = true; break; }
            }
            Assert.IsTrue(leadingVowelDetected);

            // 2. Leading vowels in the starting syllables can be disabled
            // by setting the probability to zero
            provider.WithProbability(x => x.OfLeadingVowelsInStartingSyllable(0));
            leadingVowelDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("o")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);
        }

        [TestMethod]
        public void SyllableGenerator_SettingLeadingVowelSequenceInStartingSyllable_AffectsOutput()
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
                .OfLeadingVowelsInStartingSyllable(0.5)
                .OfLeadingVowelIsSequenceInStartingSyllable(0.5)); // You can turn this off explicitly without adjusting probability settings
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
                .OfLeadingVowelsInStartingSyllable(0.5)
                .OfLeadingVowelIsSequenceInStartingSyllable(0.0)); 
            leadingVowelDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextStartingSyllable();
                if (s.StartsWith("uu")) { leadingVowelDetected = true; break; }
            }
            Assert.IsFalse(leadingVowelDetected);
        }

        [TestMethod]
        public void SyllableGenerator_WithTogglingOfLeadingConsonants_TurnsOnOrOffAsExpected()
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
            provider.WithProbability(x => x.OfLeadingConsonants(0)); // This also prevents sequences

            leadingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsFalse(leadingConsonantDetected);

            // provider.AllowLeadingConsonants(); // You can also turn this on explicitly
            // provider.WithLeadingConsonants().Chance(0.50);
            provider.WithProbability(x => x.OfLeadingConsonants(0.5));
            leadingConsonantDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("b", "c")) { leadingConsonantDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantDetected);
        }

        [TestMethod]
        public void SyllableGenerator_WithTogglingOfLeadingConsonantSequences_TurnsOnOrOffAsExpected()
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
            provider.WithProbability(x => x.OfLeadingConsonantIsSequence(0));
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
            provider.WithProbability(x => x.OfLeadingConsonantIsSequence(0.5));
            leadingConsonantSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { leadingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(leadingConsonantSequenceDetected);
        }

        [TestMethod]
        public void SyllableGenerator_WithTogglingOfVowelSequences_TurnsOnOrOffAsExpected()
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
            provider.WithProbability(x => x.OfVowelIsSequence(0));
            vowelSequenceDetected = false; // In which case we expect this variable to remain false
            
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsFalse(vowelSequenceDetected);

            // provider.WithVowelSequences().Chance(0.50); // You can also turn this on explicitly
            provider.WithProbability(x => x.OfVowelIsSequence(0.5));
            vowelSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.Contains("uu")) { vowelSequenceDetected = true; break; }
            }
            Assert.IsTrue(vowelSequenceDetected);
        }

        [TestMethod]
        public void SyllableGenerator_WithTogglingOfTrailingConsonants_TurnsOnOrOffAsExpected()
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
            provider.WithProbability(x => x.OfTrailingConsonants(0));
            trailingConsonantDetected = false; // In which case we expect this variable to remain false
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsFalse(trailingConsonantDetected);

            //provider.WithTrailingConsonants().Chance(0.50); // You can also turn this on explicitly
            provider.WithProbability(x => x.OfTrailingConsonants(0.5));
            trailingConsonantDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.EndsWith("d", "f")) { trailingConsonantDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantDetected);
        }

        [TestMethod]
        public void SyllableGenerator_WithTogglingOfLeadingConsonantSequenceSequences_TurnsOnOrOffAsExpected()
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
            provider.WithProbability(x => x.OfTrailingConsonantIsSequence(0));
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
            provider.WithProbability(x => x.OfTrailingConsonantIsSequence(0.5));
            trailingConsonantSequenceDetected = false; // In which case we expect this variable to be true again
            for (int i = 0; i < 1000; i++)
            {
                var s = provider.NextSyllable();
                if (s.StartsWith("cc")) { trailingConsonantSequenceDetected = true; break; }
            }
            Assert.IsTrue(trailingConsonantSequenceDetected);
        }


        [TestMethod]
        public void SyllableGenerator_CustomConsonantProbabilityDefined_AffectsSyllableGenerationCorrectly()
        {

            var provider = new SyllableGenerator()
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff");

            // Disable all leading and trailing consonants 
            Assert.IsTrue(EachOutputNeverContainsAnyOf(
                provider.WithProbability(x => x
                            .OfLeadingConsonants(0)
                            .OfLeadingConsonantIsSequence(0)
                            .OfTrailingConsonants(0)
                            .OfTrailingConsonantIsSequence(0)),
                    "b", "cc", "d", "ff"));

            // Consonant sequence probability doesn't matter
            // if the consonant probability is zero
            Assert.IsTrue(EachOutputNeverContainsAnyOf(
                provider.WithProbability(x => x
                            .OfLeadingConsonants(0)
                            .OfLeadingConsonantIsSequence(1)
                            .OfTrailingConsonants(0)
                            .OfTrailingConsonantIsSequence(1)),
                    "b", "cc", "d", "ff"));

            // Consonant sequence probability only matters
            // if the consonant probability is not zero

            provider.WithProbability(x => x
                            .OfLeadingConsonants(1)
                            .OfLeadingConsonantIsSequence(0)
                            .OfTrailingConsonants(1)
                            .OfTrailingConsonantIsSequence(0));

            // There should be no consonant sequences
            Assert.IsTrue(EachOutputContainsAnyOf(provider, "b", "d"));
            Assert.IsTrue(EachOutputNeverContainsAnyOf(provider, "cc", "ff"));

            provider.WithProbability(x => x
                            .OfLeadingConsonants(1)
                            .OfLeadingConsonantIsSequence(1)
                            .OfTrailingConsonants(1)
                            .OfTrailingConsonantIsSequence(1));

            // There should always be consonant sequences
            Assert.IsTrue(EachOutputNeverContainsAnyOf(provider, "b", "d"));
            Assert.IsTrue(EachOutputContainsAnyOf(provider, "cc", "ff"));

            // Test whether a value between 0 and 1 ouputs both consonants and consonant sequences
            provider.WithProbability(x => x
                            .OfLeadingConsonants(1)
                            .OfLeadingConsonantIsSequence(0.5)
                            .OfTrailingConsonants(1)
                            .OfTrailingConsonantIsSequence(0.5));

            Assert.IsTrue(AllOutputContainsAtLeastOnce(provider, "b", "d", "cc", "ff"));

            // Not all output will have a consonant
            provider.WithProbability(x => x
                            .OfLeadingConsonants(0.5)
                            .OfLeadingConsonantIsSequence(0.5)
                            .OfTrailingConsonants(0.5)
                            .OfTrailingConsonantIsSequence(0.5));

            Assert.IsTrue(AllOutputContainsAtLeastOnce(provider, "b", "d", "cc", "ff"));

        }


    }
}
