using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class ConfigurableSyllableProviderTests
    {


        [TestMethod]
        public void SyllableGeneration_WhenNoComponentsDefined_ValidOperation()
        {
            var generator = new NameGenerator();

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsNotNull(generator.Next());
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenNoComponentsDefinedExceptVowels_NamesCanStillBeGenerated()
        {
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("aeo")
                    .DisallowLeadingConsonants()
                    .DisallowLeadingConsonantSequences()
                    .DisallowVowelSequences()
                    .DisallowTrailingConsonants()
                    .DisallowTrailingConsonantSequences());

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name) && Regex.IsMatch(name,"[aeiouAEIOU]*"));
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenComponentsDefinedExceptVowels_ThrowsInvalidOperationException()
        {   
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("bb")
                    .WithTrailingConsonants("b")
                    .WithTrailingConsonantSequence("b"));

            for (int i = 0; i < 1000; i++)
            {
                Assert.ThrowsException<InvalidOperationException>(() => generator.Next());
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
                    .WithTrailingConsonantSequence("ff")
                    .WithVowelSequenceProbability(0.5)
                    .WithLeadingVowelProbability(0.25)
                    .WithLeadingConsonantSequenceProbability(0.25)
                    .WithTrailingConsonantProbability(0.25));

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
            Assert.IsTrue(foundVowel && foundVowelSequence && foundStartingConsonant && foundStartingConsonantSequence && foundEndingConsonant && foundEndingConsonantSequence); // original

        }

        [TestMethod, Timeout(10000)]
        public void SyllableGeneration_WhenNoSequencesConfigured_ProviderPermitsConfiguration()
        {
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithLeadingConsonants("srt")
                    .WithVowels("ea")
                    .WithTrailingConsonants("tz")
                    .DisallowLeadingConsonantSequences()
                    .DisallowVowelSequences()
                    .DisallowTrailingConsonantSequences())
                .UsingValidator(x => x.Invalidate("^.{,2}$"));// Invalidate names with less than 3 characters

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
        public void SyllableGeneration_WhenCustomVowelsDefined_OnlyCustomVowelsAppearInOutput()
        {

            var consonants = new string[] { "b", "c", "d", "f", "g" };

            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("a")
                    .WithLeadingConsonants(consonants)
                    .WithTrailingConsonants(consonants)
                    .DisallowVowelSequences()
                    .DisallowLeadingConsonantSequences()
                    .DisallowTrailingConsonantSequences());

            for(int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Contains("a"));
                Assert.IsFalse(Regex.IsMatch(name, "[e|i|o|u]"));
            }

        }
    }
}
