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
            provider.AddVowel("a");

            provider.UseStartingConsonants = false;
            provider.UseStartingConsonantSequences = false;
            provider.UseVowelSequences = false;
            provider.UseEndingConsonants = false;
            provider.UseEndingConsonantSequences = false;

            var generator = new NameGenerator(provider);

            for (int i = 0; i < 1000; i++)
            {
                var name = generator.Next();
                Assert.IsTrue(!string.IsNullOrEmpty(name));
            }
        }

        [TestMethod]
        public void SyllableGeneration_WhenComponentsDefinedExceptVowels_ThrowsInvalidOperationException()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddStartingConsonant("b");
            provider.AddStartingConsonantSequence("bb");
            provider.AddEndingConsonant("b");
            provider.AddEndingConsonantSequence("b");

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
            provider.AddStartingConsonant("b");
            provider.AddStartingConsonantSequence("cc");
            provider.AddEndingConsonant("d");
            provider.AddEndingConsonantSequence("ff");

            provider.VowelSequenceProbability = 0.5;
            provider.StartingVowelProbability = 0.25;
            provider.StartingConsonantSequenceProbability = 0.25;
            provider.EndingConsonantSequenceProbability = 0.25;

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

        [TestMethod]
        public void SyllableGeneration_WhenCustomVowelsDefined_OnlyCustomVowelsAppearInOutput()
        {
            var provider = new ConfigurableSyllableProvider();

            var consonants = new string[] { "b", "c", "d", "f", "g" };

            provider.AddVowel("a");
            provider.AddStartingConsonant(consonants);
            provider.AddEndingConsonant(consonants);

            provider.VowelSequenceProbability = 0;
            provider.StartingConsonantSequenceProbability = 0;
            provider.EndingConsonantSequenceProbability = 0;

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
