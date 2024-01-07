using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class SyllableGeneratorTests
    {

        // Looking to deprecate this helper method
        private static SyllableGenerator GetSyllableGeneratorWithAllComponentsDefined()
        {
            return new SyllableGenerator()
                .WithLeadingConsonants("b").Sequences("cc")
                .WithTrailingConsonants("d").Sequences("ff")
                .WithVowels("o").Sequences("uu");
        }

        private static SyllableGenerator GetSyllableGenerator(
            string vowels,
            string vowelSequences,
            string leadingConsonants,
            string leadingConsonantSequences,
            string trailingConsonants,
            string trailingConsonantSequences
        )
        {
            return new SyllableGenerator()
                .WithVowels(vowels)
                .WithVowelSequences(vowelSequences)
                .WithLeadingConsonants(leadingConsonants)
                .WithLeadingConsonantSequences(leadingConsonantSequences)
                .WithTrailingConsonants(trailingConsonants)
                .WithTrailingConsonantSequences(trailingConsonantSequences)
                .WithProbability(x => x
                    .OfVowels(1, 0.50)
                    .OfLeadingConsonants(0.50)
                    .OfTrailingConsonants(0.50)
                    .OfLeadingVowelsInStartingSyllable(0.25));
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
        public void NameGenerator_NoGraphemesWithEmptyStringAllowed_EmptyStringsGenerated()
        {
            var sg = new SyllableGenerator().AllowEmptyStrings(true);
            var sut = new NameGenerator(sg);
            Assert.AreEqual(sut.Next(), string.Empty);
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
            var sut = new SyllableGenerator().WithVowels("aeiou");
            var ng = new NameGenerator(sut);

            for (int i = 0; i < 1000; i++)
            {
                // Check that the name generator generates nothing but the vowels
                var name = ng.Next();
                Assert.IsTrue(Regex.IsMatch(name, "^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void NameGenerator_ZeroProbabilityVowelsButEmptyStringsEnabled_GenerationSuccessful()
        {
            var sut = new SyllableGenerator()
                .WithVowels("aeiou")
                .WithProbability(x => x.OfVowels(0.0))
                .AllowEmptyStrings(true);

            var ng = new NameGenerator(sut);

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(ng.Next() == string.Empty);
            }
        }

        [TestMethod]
        public void NameGenerator_ProbabilisticConsonants_GenerationSometimesNotSuccessful()
        {
            // The expectation here is that the name generator will sometimes generate valid names
            // because the probability of a leading and final consonant is not zero. However, if by chance
            // neither a leading or final consonant appears in the output, an exception will be thrown
            // because the default behaviour of the generator is to not allow empty strings.

            var sut = new SyllableGenerator()
                .WithLeadingConsonants("str")
                .WithFinalConsonants("lmn")
                .WithProbability(x => x
                    .OfLeadingConsonants(0.50)
                    .OfFinalConsonants(0.50));

            var ng = new NameGenerator(sut);

            Assert.ThrowsException<InvalidOperationException>(() => { 
                
                for(int i = 0; i < 1000; i++)
                {
                    ng.Next();
                }

            });
        }

        [TestMethod]
        public void NameGenerator_ProbabilisticConsonantsButEmptyStringsAllowed_GenerationAlwaysSuccessful()
        {
            // If neither a leading or final consonant appears in the output, no exception will be thrown
            // because the generator has been instructed to allow empty strings.

            var sut = new SyllableGenerator()
                .WithLeadingConsonants("str")
                .WithFinalConsonants("lmn")
                .WithProbability(x => x
                    .OfLeadingConsonants(0.50)
                    .OfFinalConsonants(0.50))
                .AllowEmptyStrings(true);

            var ng = new NameGenerator(sut);

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsNotNull(ng.Next());
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
            var sut = GetConsonantOnlyGenerator()
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0, 0.5)
                    .OfTrailingConsonants(1.0, 0.5));

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsTrue(sut.NextEndingSyllable().Length > 0);
            }
        }

        [TestMethod]
        public void SyllableGenerator_VowelSequenceWithZeroProbability_GenerationFailure()
        {
            // Defining at least one vowel sequence, but no individual vowels also shouldn't work if
            // the probability of vowels turning into sequences is not set to 100%
            var sut = new SyllableGenerator()
                .WithVowelSequences("aa")
                .WithProbability(x => x
                    .OfVowels(1.0, 0));

            var names = new NameGenerator(sut);

            Assert.ThrowsException<InvalidOperationException>(() => // Expecting some to fail
            {
                for (int i = 0; i < 1000; i++)
                {
                    names.Next();
                }
            });
            
        }

        [TestMethod]
        public void SyllableGenerator_VowelSequenceWithGuaranteedProbability1_GuaranteedGenerationSuccess()
        {
            // Defining at least one vowel sequence, but no individual vowels also should work if
            // the probability of vowels turning into sequences is 100%
            var sut = new SyllableGenerator()
                .WithVowelSequences("aa")
                .WithProbability(x => x
                    .OfVowels(1.0, 1.0));

            var names = new NameGenerator(sut);

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(string.IsNullOrEmpty(names.Next()));
            }
        }

        [TestMethod]
        public void SyllableGenerator_WithCustomGraphemes1_AllGraphemesAppearInOutput()
        {
            // In this test we define one instance of a vowel, vowel sequence
            // leading consonant, leading consonant sequence, trailing consonant
            // and trailing consonant sequence, then check that each instance occurs
            // at least once during generation.

            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff");

            var discovered = new Dictionary<string, bool>()
            {
                { "a",  false },
                { "ee", false },
                { "b",  false },
                { "cc", false },
                { "d",  false },
                { "ff", false }
            };

            for (int i = 0; i < 1000; i++)
            {
                var syllable = sut.NextStartingSyllable();
                discovered["a"]  |= syllable.Contains("a");
                discovered["ee"] |= syllable.Contains("ee");
                discovered["b"]  |= syllable.Contains("b");
                discovered["cc"] |= syllable.Contains("cc");
                discovered["d"]  |= syllable.Contains("d");
                discovered["ff"] |= syllable.Contains("ff");
            }

            Assert.IsTrue(discovered.Values.All(x => x));
        }

        [TestMethod]
        public void SyllableGenerator_WithCustomGraphemes2_AllGraphemesAppearInOutput()
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff");

            var discovered = new Dictionary<string, bool>()
            {
                { "a",  false },
                { "ee", false },
                { "b",  false },
                { "cc", false },
                { "d",  false },
                { "ff", false }
            };

            for (int i = 0; i < 1000; i++)
            {
                var syllable = sut.NextSyllable();
                discovered["a"]  |= syllable.Contains("a");
                discovered["ee"] |= syllable.Contains("ee");
                discovered["b"]  |= syllable.Contains("b");
                discovered["cc"] |= syllable.Contains("cc");
                discovered["d"]  |= syllable.Contains("d");
                discovered["ff"] |= syllable.Contains("ff");
            }

            Assert.IsTrue(discovered.Values.All(x => x));
        }

        [TestMethod]
        public void SyllableGenerator_WithCustomGraphemes3_AllGraphemesAppearInOutput()
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff");

            var discovered = new Dictionary<string, bool>()
            {
                { "a",  false },
                { "ee", false },
                { "b",  false },
                { "cc", false },
                { "d",  false },
                { "ff", false }
            };

            for (int i = 0; i < 1000; i++)
            {
                var syllable = sut.NextEndingSyllable();
                discovered["a"]  |= syllable.Contains("a");
                discovered["ee"] |= syllable.Contains("ee");
                discovered["b"]  |= syllable.Contains("b");
                discovered["cc"] |= syllable.Contains("cc");
                discovered["d"]  |= syllable.Contains("d");
                discovered["ff"] |= syllable.Contains("ff");
            }

            Assert.IsTrue(discovered.Values.All(x => x));
        }


        [TestMethod]
        public void NameGenerator_WithCustomComponents_AllComponentsAppearInNameGeneratorOutput()
        {
            // Same as SyllableGenerator_WithCustomGraphemesX_AllGraphemesAppearInOutput
            // but checking output of a NameGenerator

            var sut = new NameGenerator(GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff"));

            var discovered = new Dictionary<string, bool>()
            {
                { "a",  false },
                { "ee", false },
                { "b",  false },
                { "cc", false },
                { "d",  false },
                { "ff", false }
            };

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next();
                discovered["a"]  |= name.Contains("a");
                discovered["ee"] |= name.Contains("ee");
                discovered["b"]  |= name.Contains("b");
                discovered["cc"] |= name.Contains("cc");
                discovered["d"]  |= name.Contains("d");
                discovered["ff"] |= name.Contains("ff");
            }

            Assert.IsTrue(discovered.Values.All(x => x));

        }

        [TestMethod, Timeout(10000)]
        public void SyllableGenerator_WithNoSequencesDefined_NameGeneratorStillGeneratesNames()
        {
            // It is valid for a name generator to use a provider with no sequences defined

            var sut = new SyllableGenerator()
                    .WithLeadingConsonants("srt")
                    .WithVowels("ea")
                    .WithTrailingConsonants("tz")
                    .WithProbability(x => x.OfLeadingConsonants(1.0));

            var generator = new NameGenerator()
                .UsingSyllables(sut)
                .DoNotAllow("^.{,2}$");// Invalidate names with less than 3 characters

            for (int i = 0; i < 10000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Length > 2);
            }

        }

        [TestMethod]
        [DataRow(0.0, false, true)]
        [DataRow(0.5, true, true)]
        [DataRow(1.0, true, false)]
        public void SyllableGenerator_CustomLeadingVowelProbability_AffectsOutput(
            double leadingVowelProbability, 
            bool isLeadingVowelExpected, 
            bool isLeadingConsonantExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0)
                    .OfLeadingVowelsInStartingSyllable(leadingVowelProbability));

            var leadingVowelsDetected = false;
            var leadingConsonantsDetected = false;

            for(int i = 0; i < 1000; i++)
            {
                var s = sut.NextStartingSyllable();
                leadingVowelsDetected |= Regex.IsMatch(s, "^[aieou]");
                leadingConsonantsDetected |= Regex.IsMatch(s, "^[^aieou]"); // note the negation in the regex
            }

            Assert.IsTrue(leadingVowelsDetected == isLeadingVowelExpected);
            Assert.IsTrue(leadingConsonantsDetected == isLeadingConsonantExpected);
        }

        [TestMethod]
        [DataRow(0.0, false, true)]
        [DataRow(0.5, true, true)]
        [DataRow(1.0, true, false)]
        public void SyllableGenerator_CustomLeadingVowelSequenceProbability_AffectsOutput(
            double leadingVowelSequenceProbability,
            bool outputWithLeadingVowelSequenceExpected,
            bool outputWithoutLeadingVowelSequenceExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0)
                    .OfLeadingVowelsInStartingSyllable(1.0, leadingVowelSequenceProbability));

            var leadingVowelSequenceDetected = false;
            var otherLeadingGraphemeDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextStartingSyllable();
                leadingVowelSequenceDetected |= Regex.IsMatch(s, "^ee");
                otherLeadingGraphemeDetected |= !Regex.IsMatch(s, "^ee");
            }

            Assert.IsTrue(leadingVowelSequenceDetected == outputWithLeadingVowelSequenceExpected);
            Assert.IsTrue(otherLeadingGraphemeDetected == outputWithoutLeadingVowelSequenceExpected);
        }

       
        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomLeadingConsonantProbability_AffectsOutput(
            double leadingConsonantProbability,
            bool isLeadingConsonantExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x
                    .OfLeadingConsonants(leadingConsonantProbability));

            var leadingConsonantsDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                leadingConsonantsDetected |= Regex.IsMatch(s, "^b");
            }

            Assert.IsTrue(leadingConsonantsDetected == isLeadingConsonantExpected);
        }

        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomLeadingConsonantSequenceProbability_AffectsOutput(
            double leadingConsonantSequenceProbability,
            bool isLeadingConsonantSequenceExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x
                    .OfLeadingConsonants(1.0, leadingConsonantSequenceProbability));

            var leadingConsonantsDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                leadingConsonantsDetected |= Regex.IsMatch(s, "^cc");
            }

            Assert.IsTrue(leadingConsonantsDetected == isLeadingConsonantSequenceExpected);
        }

        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomVowelProbability_AffectsOutput(
            double vowelProbability,
            bool isVowelExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .AllowEmptyStrings(true)
                .WithProbability(x => x.OfVowels(vowelProbability));

            var vowelsDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                vowelsDetected |= Regex.IsMatch(s, "a");
            }

            Assert.IsTrue(vowelsDetected == isVowelExpected);
        }

        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomVowelSequenceProbability_AffectsOutput(
            double vowelSequenceProbability,
            bool isVowelSequenceExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x.OfVowels(1.0, vowelSequenceProbability));

            var vowelSequenceDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                vowelSequenceDetected |= Regex.IsMatch(s, "ee");
            }

            Assert.IsTrue(vowelSequenceDetected == isVowelSequenceExpected);
        }


        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomTrailingConsonantProbability_AffectsOutput(
            double trailingConsonantProbability,
            bool isTrailingConsonantExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x.OfTrailingConsonants(trailingConsonantProbability));

            var trailingConsonantsDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                trailingConsonantsDetected |= Regex.IsMatch(s, "d$");
            }

            Assert.IsTrue(trailingConsonantsDetected == isTrailingConsonantExpected);
        }

        [TestMethod]
        [DataRow(0.0, false)]
        [DataRow(0.5, true)]
        [DataRow(1.0, true)]
        public void SyllableGenerator_CustomTrailingConsonantSequenceProbability_AffectsOutput(
            double trailingConsonantSequenceProbability,
            bool isTrailingConsonantSequenceExpected
        )
        {
            var sut = GetSyllableGenerator("a", "ee", "b", "cc", "d", "ff")
                .WithProbability(x => x
                    .OfTrailingConsonants(1.0, trailingConsonantSequenceProbability));

            var trailingConsonantsDetected = false;

            for (int i = 0; i < 1000; i++)
            {
                var s = sut.NextSyllable();
                trailingConsonantsDetected |= Regex.IsMatch(s, "ff$");
            }

            Assert.IsTrue(trailingConsonantsDetected == isTrailingConsonantSequenceExpected);
        }

    }
}
