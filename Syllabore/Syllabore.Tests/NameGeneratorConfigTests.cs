using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorConfigTests
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
                    .UsingTransformer(x => x
                        .Select(3).Chance(0.5))
                    .UsingFilter(x => x
                        .DoNotAllowPattern(
                            "zzzy",
                            "abcd"))
                    .LimitRetries(100)
                    .UsingSyllableCount(4, 10);
        }

        [TestMethod]
        public void ConfigurationFile_Serialization_Succeeds()
        {
            var g = InitializeNameGenerator();
            NameGeneratorConfig.Save(g, "test.txt");
            Assert.IsTrue(File.Exists("test.txt"));
        }

        [TestMethod]
        public void ConfigurationFile_Deserialization_Succeeds()
        {
            var g = InitializeNameGenerator();
            NameGeneratorConfig.Save(g, "test.txt");

            var g2 = NameGeneratorConfig.Load("test.txt");
            Assert.IsNotNull(g2);
            Assert.IsTrue(g != g2);

            // Compare the deserialized instance with pre-deserialized instance
            Assert.IsTrue(g.MaximumRetries == g2.MaximumRetries);
            Assert.IsTrue(g.MaximumSyllables == g2.MaximumSyllables);
            Assert.IsTrue(g.MinimumSyllables == g2.MinimumSyllables);
            Assert.IsTrue(g.Transformer.TransformChance == g2.Transformer.TransformChance);

            var t1 = (NameTransformer)g.Transformer; // The default type
            var t2 = (NameTransformer)g2.Transformer;

            Assert.AreEqual(t1.SelectionLimit, t2.SelectionLimit);

            var p1 = (SyllableProvider)g.Provider;  // The default type
            var p2 = (SyllableProvider)g2.Provider;

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

            var f = (NameFilter)g.Filter;
            var f2 = (NameFilter)g2.Filter;

            Assert.IsNotNull(f);
            Assert.IsNotNull(f.Constraints);
            Assert.IsNotNull(f2);
            Assert.IsNotNull(f2.Constraints);
            Assert.IsTrue(f.Constraints.UnorderedListEquals(f2.Constraints));

        }

    }

}
