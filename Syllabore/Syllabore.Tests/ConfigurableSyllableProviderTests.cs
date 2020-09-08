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
        public void SyllableGeneration_WhenNoComponentsDefined_ThrowsInvalidOperationException()
        {
            var provider = new ConfigurableSyllableProvider();
            var generator = new NameGenerator(provider);

            for (int i = 0; i < 1000; i++)
            {
                Assert.ThrowsException<InvalidOperationException>(() => generator.Next());
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenNoComponentsDefinedExceptVowels_NamesCanStillBeGenerated()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddVowel("a","e,","o");

            provider.UseLeadingConsonants = false;
            provider.UseLeadingConsonantSequences = false;
            provider.UseVowelSequences = false;
            provider.UseTrailingConsonants = false;
            provider.UseTrailingConsonantSequences = false;

            var generator = new NameGenerator(provider);

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name) && Regex.IsMatch(name,"[aeiouAEIOU]*"));
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenComponentsDefinedExceptVowels_ThrowsInvalidOperationException()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddLeadingConsonant("b");
            provider.AddLeadingConsonantSequence("bb");
            provider.AddTrailingConsonant("b");
            provider.AddTrailingConsonantSequence("b");
            
            var generator = new NameGenerator(provider);

            for (int i = 0; i < 1000; i++)
            {
                Assert.ThrowsException<InvalidOperationException>(() => generator.Next());
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenAllComponentsDefined_AllComponentsAppearInOutput()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddVowel("a");
            provider.AddVowelSequence("ee");
            provider.AddLeadingConsonant("b");
            provider.AddLeadingConsonantSequence("cc");
            provider.AddTrailingConsonant("d");
            provider.AddTrailingConsonantSequence("ff");

            provider.VowelSequenceProbability = 0.5;
            provider.LeadingVowelProbability = 0.25;
            provider.LeadingConsonantSequenceProbability = 0.25;
            provider.TrailingConsonantSequenceProbability = 0.25;

            var generator = new NameGenerator(provider);

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

            Assert.IsTrue(foundVowel && foundVowelSequence && foundStartingConsonant && foundStartingConsonantSequence && foundEndingConsonant && foundEndingConsonantSequence);

        }

        [TestMethod, Timeout(10000)]
        public void SyllableGeneration_WhenNoSequencesConfigured_ProviderPermitsConfiguration()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddLeadingConsonant("s", "r", "t");
            provider.AddVowel("e","a");
            provider.AddTrailingConsonant("t","z");

            provider.UseLeadingConsonantSequences = false;
            provider.UseVowelSequences = false;
            provider.UseTrailingConsonantSequences = false;

            var validator = new ConfigurableNameValidator();
            validator.AddConstraintAsRegex("^.{,2}$"); // Invalidate names with less than 3 characters

            var generator = new NameGenerator(provider, validator);

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
            var provider = new ConfigurableSyllableProvider();

            var consonants = new string[] { "b", "c", "d", "f", "g" };

            provider.AddVowel("a");
            provider.AddLeadingConsonant(consonants);
            provider.AddTrailingConsonant(consonants);

            provider.VowelSequenceProbability = 0;
            provider.LeadingConsonantSequenceProbability = 0;
            provider.TrailingConsonantSequenceProbability = 0;

            var generator = new NameGenerator(provider);

            for(int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(name.Contains("a"));
                Assert.IsFalse(Regex.IsMatch(name, "[e|i|o|u]"));
            }

        }
    }
}
