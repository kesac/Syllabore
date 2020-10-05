using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Syllabore.Tests
{
    [TestClass]
    public class ConfigurationFileTests
    {
        [TestMethod]
        public void ConfigurationFile_SavingAndLoading_Succeeds()
        {
            var g = new NameGenerator()
                .UsingValidator(x => x
                    .DoNotAllowPattern(
                        "zzzy",
                        "abcd"));

            ConfigurationFile.Save(g, "test.txt");
            Assert.IsTrue(File.Exists("test.txt"));

            var g2 = ConfigurationFile.Load("test.txt");
            Assert.IsNotNull(g2);
            Assert.IsTrue(g != g2); // Ensure they are not the same instance

            Assert.IsTrue(g.MaximumRetries == g2.MaximumRetries);
            Assert.IsTrue(g.MaximumSyllables == g2.MaximumSyllables);
            Assert.IsTrue(g.MinimumSyllables == g2.MinimumSyllables);
            Assert.IsTrue(g.MutationProbability == g2.MutationProbability);

            // Components of a syllable
            Assert.IsTrue(g.Provider.LeadingConsonants.UnorderedListEquals(g2.Provider.LeadingConsonants));
            Assert.IsTrue(g.Provider.LeadingConsonantSequences.UnorderedListEquals(g2.Provider.LeadingConsonantSequences));
            Assert.IsTrue(g.Provider.Vowels.UnorderedListEquals(g2.Provider.Vowels));
            Assert.IsTrue(g.Provider.VowelSequences.UnorderedListEquals(g2.Provider.VowelSequences));
            Assert.IsTrue(g.Provider.TrailingConsonants.UnorderedListEquals(g2.Provider.TrailingConsonants));
            Assert.IsTrue(g.Provider.TrailingConsonantSequences.UnorderedListEquals(g2.Provider.TrailingConsonantSequences));

            // Usage flags
            Assert.IsTrue(g.Provider.UseLeadingConsonants == g2.Provider.UseLeadingConsonants);
            Assert.IsTrue(g.Provider.UseLeadingConsonantSequences == g2.Provider.UseLeadingConsonantSequences);
            Assert.IsTrue(g.Provider.UseStartingSyllableLeadingVowels == g2.Provider.UseStartingSyllableLeadingVowels);
            Assert.IsTrue(g.Provider.UseVowelSequences == g2.Provider.UseVowelSequences);
            Assert.IsTrue(g.Provider.UseTrailingConsonants == g2.Provider.UseTrailingConsonants);
            Assert.IsTrue(g.Provider.UseTrailingConsonantSequences == g2.Provider.UseTrailingConsonantSequences);

            // Probability of a component showing up in a syllable
            Assert.IsTrue(g.Provider.Probability.LeadingConsonant == g2.Provider.Probability.LeadingConsonant);
            Assert.IsTrue(g.Provider.Probability.LeadingConsonantSequence == g2.Provider.Probability.LeadingConsonantSequence);
            Assert.IsTrue(g.Provider.Probability.VowelSequence == g2.Provider.Probability.VowelSequence);
            Assert.IsTrue(g.Provider.Probability.StartingSyllableLeadingVowel == g2.Provider.Probability.StartingSyllableLeadingVowel); 
            Assert.IsTrue(g.Provider.Probability.StartingSyllableLeadingVowelSequence == g2.Provider.Probability.StartingSyllableLeadingVowelSequence);
            Assert.IsTrue(g.Provider.Probability.TrailingConsonant == g2.Provider.Probability.TrailingConsonant);
            Assert.IsTrue(g.Provider.Probability.TrailingConsonantSequence == g2.Provider.Probability.TrailingConsonantSequence);

            // Invalid regular expressions
            Assert.IsNotNull(g.Validator);
            Assert.IsNotNull(g.Validator.InvalidPatterns);
            Assert.IsNotNull(g2.Validator);
            Assert.IsNotNull(g2.Validator.InvalidPatterns);

            Assert.IsTrue(g.Validator.InvalidPatterns.UnorderedListEquals(g2.Validator.InvalidPatterns));

        }

        

    }
}
