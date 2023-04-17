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
    public class NameGeneratorSerializationTests
    {
        private NameGenerator InitializeNameGenerator()
        {
            return new NameGenerator()
                    .UsingSyllables(x => x
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
            var f = new NameGeneratorSerializer();
            f.Serialize(g, "test.txt");
            Assert.IsTrue(File.Exists("test.txt"));
        }

        [TestMethod]
        public void ConfigurationFile_CanSerializeNonDefaultTypes()
        {
            var output = "NameGenerator_WithSyllableSet.json";

            var p = new SyllableSet(2, 16, 2)
                    .WithProvider(x => x
                        .WithConsonants("str")
                        .WithVowels("aeiou"))
                    .WithStartingSyllable("ra")
                    .WithMiddleSyllable("ro")
                    .WithEndingSyllable("ri");
                        
            var g = InitializeNameGenerator().UsingSyllables(p);

            var n = new NameGeneratorSerializer().UsingProviderType(typeof(SyllableSet));

            n.Serialize(g, output);
            Assert.IsTrue(File.Exists(output));

            var g2 = n.Deserialize(output);

            Assert.IsNotNull(g2.Provider);
            Assert.IsTrue(g2.Provider is SyllableSet);

            var s = g2.Provider as SyllableSet;

            for (int i = 0; i < 50; i++)
            {
                g2.Next();
            }

            Assert.IsTrue(s.StartingSyllables.Count == 2);
            Assert.IsTrue(s.MiddleSyllables.Count == 16);
            Assert.IsTrue(s.EndingSyllables.Count == 2);

        }

        [TestMethod]
        public void ConfigurationFile_Deserialization_Succeeds()
        {
            var g = InitializeNameGenerator();
            var n = new NameGeneratorSerializer();

            n.Serialize(g, "test.txt");

            var g2 = n.Deserialize("test.txt");

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

            var p1 = (SyllableGenerator)g.Provider;  // The default type
            var p2 = (SyllableGenerator)g2.Provider;

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
            Assert.IsTrue(p1.Probability.ChanceLeadingConsonantIsSequence == p2.Probability.ChanceLeadingConsonantIsSequence);
            Assert.IsTrue(p1.Probability.ChanceVowelIsSequence == p2.Probability.ChanceVowelIsSequence);
            Assert.IsTrue(p1.Probability.ChanceStartingSyllableLeadingVowelExists == p2.Probability.ChanceStartingSyllableLeadingVowelExists);
            Assert.IsTrue(p1.Probability.ChanceStartingSyllableLeadingVowelIsSequence == p2.Probability.ChanceStartingSyllableLeadingVowelIsSequence);
            Assert.IsTrue(p1.Probability.ChanceTrailingConsonantExists == p2.Probability.ChanceTrailingConsonantExists);
            Assert.IsTrue(p1.Probability.ChanceTrailingConsonantIsSequence == p2.Probability.ChanceTrailingConsonantIsSequence);

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
