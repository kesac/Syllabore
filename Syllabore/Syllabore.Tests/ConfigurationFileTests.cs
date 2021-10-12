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
        private NameGenerator InitializeNameGenerator()
        {
            return new NameGenerator()
                    .UsingProvider(x => x
                        .WithLeadingConsonants("bcd")
                        .WithLeadingConsonantSequences("dr")
                        .WithVowels("aieou")
                        .WithVowelSequences("ey")
                        .WithTrailingConsonants("trs")
                        .WithTrailingConsonantSequences("mn"))
                    .UsingMutator(x => x
                        .WithMutationCount(3))
                    .UsingValidator(x => x
                        .DoNotAllowPattern(
                            "zzzy",
                            "abcd"))
                    .LimitMutationChance(0.50)
                    .LimitRetries(100)
                    .LimitSyllableCount(4, 10);
        }

        [TestMethod]
        public void ConfigurationFile_Serialization_Succeeds()
        {
            var g = InitializeNameGenerator();
            ConfigurationFile.Save(g, "test.txt");
            Assert.IsTrue(File.Exists("test.txt"));
        }

        [TestMethod]
        public void ConfigurationFile_Deserialization_Succeeds()
        {
            var g = InitializeNameGenerator();
            ConfigurationFile.Save(g, "test.txt");

            var g2 = ConfigurationFile.Load("test.txt");
            Assert.IsNotNull(g2);
            Assert.IsTrue(g != g2);

            // Compare the deserialized instance with pre-deserialized instance
            Assert.IsTrue(g.MaximumRetries == g2.MaximumRetries);
            Assert.IsTrue(g.MaximumSyllables == g2.MaximumSyllables);
            Assert.IsTrue(g.MinimumSyllables == g2.MinimumSyllables);
            Assert.IsTrue(g.MutationProbability == g2.MutationProbability);
            Assert.AreEqual(g.Mutator.MutationLimit, g2.Mutator.MutationLimit);

            var p1 = g.Provider;
            var p2 = g2.Provider;

            // Components of a syllable
            Assert.IsTrue(p1.LeadingConsonants.UnorderedListEquals(p2.LeadingConsonants));
            Assert.IsTrue(p1.LeadingConsonantSequences.UnorderedListEquals(p2.LeadingConsonantSequences));
            Assert.IsTrue(p1.Vowels.UnorderedListEquals(p2.Vowels));
            Assert.IsTrue(p1.VowelSequences.UnorderedListEquals(p2.VowelSequences));
            Assert.IsTrue(p1.TrailingConsonants.UnorderedListEquals(p2.TrailingConsonants));
            Assert.IsTrue(p1.TrailingConsonantSequences.UnorderedListEquals(p2.TrailingConsonantSequences));

            // Usage flags
            Assert.IsTrue(p1.LeadingConsonantsAllowed == p2.LeadingConsonantsAllowed);
            Assert.IsTrue(p1.LeadingConsonantSequencesAllowed == p2.LeadingConsonantSequencesAllowed);
            Assert.IsTrue(p1.LeadingVowelForStartingSyllableAllowed == p2.LeadingVowelForStartingSyllableAllowed);
            Assert.IsTrue(p1.VowelSequencesAllowed == p2.VowelSequencesAllowed);
            Assert.IsTrue(p1.TrailingConsonantsAllowed == p2.TrailingConsonantsAllowed);
            Assert.IsTrue(p1.TrailingConsonantSequencesAllowed == p2.TrailingConsonantSequencesAllowed);

            // Probability of a component showing up in a syllable
            Assert.IsTrue(p1.Probability.ChanceLeadingConsonantExists == p2.Probability.ChanceLeadingConsonantExists);
            Assert.IsTrue(p1.Probability.ChanceLeadingConsonantBecomesSequence == p2.Probability.ChanceLeadingConsonantBecomesSequence);
            Assert.IsTrue(p1.Probability.ChanceVowelBecomesSequence == p2.Probability.ChanceVowelBecomesSequence);
            Assert.IsTrue(p1.Probability.StartingSyllable.ChanceLeadingVowelExists == p2.Probability.StartingSyllable.ChanceLeadingVowelExists);
            Assert.IsTrue(p1.Probability.StartingSyllable.ChanceLeadingVowelBecomesSequence == p2.Probability.StartingSyllable.ChanceLeadingVowelBecomesSequence);
            Assert.IsTrue(p1.Probability.ChanceTrailingConsonantExists == p2.Probability.ChanceTrailingConsonantExists);
            Assert.IsTrue(p1.Probability.ChanceTrailingConsonantBecomesSequence == p2.Probability.ChanceTrailingConsonantBecomesSequence);

            // Invalid regular expressions
            Assert.IsNotNull(g.Validator);
            Assert.IsNotNull(g.Validator.InvalidPatterns);
            Assert.IsNotNull(g2.Validator);
            Assert.IsNotNull(g2.Validator.InvalidPatterns);
            Assert.IsTrue(g.Validator.InvalidPatterns.UnorderedListEquals(g2.Validator.InvalidPatterns));

        }

    }

}
