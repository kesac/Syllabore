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
        public void SyllableGeneration_WhenOnlyVowelsDefined_NamesCanStillBeGenerated()
        {
            var generator = new NameGenerator()
                .UsingProvider(x => x.WithVowels("aeo"));

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name) && Regex.IsMatch(name,"^[aeiouAEIOU]+$"));
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenNoVowelsDefined_ThrowsInvalidOperationException()
        {
            // No vowel or vowel sequences defined shouldn't work
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

            // Defining at least one vowel sequence, but no individual vowels also shouldn't work if
            // the probability of vowel sequences is not set to 100%
            provider.WithVowelSequences("aa");

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

        [TestMethod]
        public void SyllableGeneration_WhenAllComponentsDefined_AllComponentsAppearInOutput()
        {
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

            for (int i = 0; i < 10000; i++)
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
        public void SyllableGeneration_WhenNoSequencesConfigured_Allowed()
        {
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonants("srt")
                    .WithVowels("ea")
                    .WithTrailingConsonants("tz"))
                .UsingValidator(x => x.InvalidateRegex("^.{,2}$"));// Invalidate names with less than 3 characters

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
        public void SyllableGeneration_WhenOnlySequencesConfigured_Allowed()
        {

            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonantSequences("sr")
                    .WithVowelSequences("ea")
                    .WithTrailingConsonantSequences("bz")
                    .WithProbability(x => x.OfVowelSequences(1.0))
                    .DisallowStartingSyllableLeadingVowels())
                .UsingValidator(x => x.InvalidateRegex("^.{,2}$"));// Invalidate names with less than 3 characters

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

        [TestMethod]
        public void SyllableGeneration_WhenCustomVowelsDefined_OnlyCustomVowelsAppearInOutput()
        {

            var consonants = new string[] { "b", "c", "d", "f", "g" };

            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("a")
                    .WithLeadingConsonants(consonants)
                    .WithTrailingConsonants(consonants));

            for(int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Contains("a"));
                Assert.IsFalse(Regex.IsMatch(name, "[e|i|o|u]"));
            }

        }
    }
}
