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
            Assert.IsTrue(p1.UseLeadingConsonants == p2.UseLeadingConsonants);
            Assert.IsTrue(p1.UseLeadingConsonantSequences == p2.UseLeadingConsonantSequences);
            Assert.IsTrue(p1.UseStartingSyllableLeadingVowels == p2.UseStartingSyllableLeadingVowels);
            Assert.IsTrue(p1.UseVowelSequences == p2.UseVowelSequences);
            Assert.IsTrue(p1.UseTrailingConsonants == p2.UseTrailingConsonants);
            Assert.IsTrue(p1.UseTrailingConsonantSequences == p2.UseTrailingConsonantSequences);

            // Probability of a component showing up in a syllable
            Assert.IsTrue(p1.ChanceConsonantBeginsSyllable == p2.ChanceConsonantBeginsSyllable);
            Assert.IsTrue(p1.ChanceConsonantBeginsSyllableAndIsSequence == p2.ChanceConsonantBeginsSyllableAndIsSequence);
            Assert.IsTrue(p1.ChanceVowelIsSequence == p2.ChanceVowelIsSequence);
            Assert.IsTrue(p1.ChanceVowelBeginsStartingSyllable == p2.ChanceVowelBeginsStartingSyllable);
            Assert.IsTrue(p1.ChanceVowelBeginsStartingSyllableAndIsSequence == p2.ChanceVowelBeginsStartingSyllableAndIsSequence);
            Assert.IsTrue(p1.ChanceConsonantEndsSyllable == p2.ChanceConsonantEndsSyllable);
            Assert.IsTrue(p1.ChanceConsonantEndsSyllableAndIsSequence == p2.ChanceConsonantEndsSyllableAndIsSequence);

            // Invalid regular expressions
            Assert.IsNotNull(g.Validator);
            Assert.IsNotNull(g.Validator.InvalidPatterns);
            Assert.IsNotNull(g2.Validator);
            Assert.IsNotNull(g2.Validator.InvalidPatterns);
            Assert.IsTrue(g.Validator.InvalidPatterns.UnorderedListEquals(g2.Validator.InvalidPatterns));


        }

        

    }
}
