using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameGeneratorSerializationTests
    {
        private readonly NameGenerator _defaultNameGenerator;
        private readonly string _syllableSetFile = "NameGenerator_WithSyllableSet.json";
        private readonly string _defaultFile = "NameGenerator_Default.json";

        public NameGeneratorSerializationTests()
        {
            _defaultNameGenerator = new NameGenerator()
                .UsingSyllables(x => x
                    .WithLeadingConsonants("bcd")
                    .WithLeadingConsonantSequences("dr")
                    .WithVowels("aieou")
                    .WithVowelSequences("ey")
                    .WithTrailingConsonants("trs")
                    .WithTrailingConsonantSequences("mn"))
                .DoNotAllow(
                    "zzzy",
                    "abcd")
                .UsingTransform(0.5, new TransformSet()
                    .RandomlySelect(2)
                    .WithTransform(x => x.AppendSyllable("tar"))
                    .WithTransform(x => x.InsertSyllable(0, "arc"))
                    .WithTransform(x => x.ReplaceSyllable(0, "neo")))
                .UsingSyllableCount(3)
                .LimitRetries(100)
                .UsingSyllableCount(4, 10);
        }

        [TestMethod]
        public void Serializer_DefaultType1_SerializationSucceeds()
        {
            var sut = new NameGeneratorSerializer();
            sut.Serialize(_defaultNameGenerator, _defaultFile);
            Assert.IsTrue(File.Exists(_defaultFile));
        }

        [TestMethod]
        public void Serializer_DefaultType2_DeserializationPreservesValueTypeProperties()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);

            Assert.IsNotNull(g2);

            // Compare the deserialized instance with pre-deserialized instance
            var nameGeneratorType = typeof(NameGenerator);
            foreach(var property in nameGeneratorType.GetProperties().Where(x => x.PropertyType.IsValueType))
            {
                Assert.IsTrue(property.GetValue(_defaultNameGenerator).Equals(property.GetValue(g2)));
            }

        }

        [TestMethod]
        public void Serializer_DefaultType3_DeserializationPreservesTransformer()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);
            var t1 = _defaultNameGenerator.Transformer as TransformSet; // The default type
            var t2 = g2.Transformer as TransformSet;

            Assert.AreEqual(t1.RandomSelectionCount, t2.RandomSelectionCount);

        }

        [TestMethod]
        public void Serializer_DefaultType4_DeserializationPreservesSyllableGeneratorGraphemes()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);

            var p1 = _defaultNameGenerator.Provider as SyllableGenerator;  // The default type
            var p2 = g2.Provider as SyllableGenerator;

            // Components of a syllable
            var syllableGeneratorType = typeof(SyllableGenerator);
            var syllableGeneratorProperties = syllableGeneratorType.GetProperties();

            // Comparing grapheme lists like vowels, leading consonants, sequences, etc.
            foreach (var property in syllableGeneratorProperties.Where(x => x.PropertyType == typeof(List<Symbol>)))
            {
                var list1 = (List<Symbol>)property.GetValue(p1);
                var list2 = (List<Symbol>)property.GetValue(p2);
                Assert.IsTrue(list1.UnorderedListEquals(list2));
            }

        }

        [TestMethod]
        public void Serializer_DefaultType5_DeserializationPreservesSyllableGeneratorUsageFlags()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);

            var p1 = _defaultNameGenerator.Provider as SyllableGenerator;  // The default type
            var p2 = g2.Provider as SyllableGenerator;

            var syllableGeneratorType = typeof(SyllableGenerator);
            var syllableGeneratorProperties = syllableGeneratorType.GetProperties();

            // Comparing usage flags like  LeadingConsonantsAllowed, VowelSequencesAllowed, etc.
            foreach (var property in syllableGeneratorProperties.Where(x => x.PropertyType == typeof(bool)))
            {
                Assert.IsTrue(property.GetValue(p1).Equals(property.GetValue(p2)));
            }

        }

        [TestMethod]
        public void Serializer_DefaultType6_DeserializationPreservesSyllableGeneratorProbabilities()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);

            var p1 = _defaultNameGenerator.Provider as SyllableGenerator;  // The default type
            var p2 = g2.Provider as SyllableGenerator;

            // Comparing probabilities like ChanceVowelExists, ChanceLeadingConsonantExists, etc.
            var probabilityType = typeof(GeneratorProbability);
            foreach (var property in probabilityType.GetProperties().Where(x => x.PropertyType == typeof(double?)))
            {
                var probability1 = (double?)property.GetValue(p1.Probability);
                var probability2 = (double?)property.GetValue(p2.Probability);

                if (probability1 == null)
                {
                    Assert.IsTrue(probability2 == null);
                }
                else
                {
                    Assert.IsTrue(property.GetValue(p1.Probability).Equals(property.GetValue(p2.Probability)));
                }
            }
        }

        [TestMethod]
        public void Serializer_DefaultType7_DeserializationPreservesFilters()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);
            var f = (NameFilter)_defaultNameGenerator.Filter;
            var f2 = (NameFilter)g2.Filter;

            Assert.IsTrue(f.Constraints.UnorderedListEquals(f2.Constraints));

        }

        [TestMethod]
        public void Serializer_NonDefaultType1_SerializationSucceeds()
        {
            var p = new SyllableSet(2, 16, 2)
                    .WithGenerator(x => x
                        .WithConsonants("str")
                        .WithVowels("aeiou"))
                    .WithStartingSyllable("ra")
                    .WithMiddleSyllable("ro")
                    .WithEndingSyllable("ri");
                        
            var g = new NameGenerator(p)
                .UsingSyllableCount(3);

            var n = new NameGeneratorSerializer()
                .UsingProviderType(typeof(SyllableSet));

            n.Serialize(g, _syllableSetFile);
            Assert.IsTrue(File.Exists(_syllableSetFile));
        }

        [TestMethod]
        public void Serializer_NonDefaultType2_DeserializationSucceeds()
        {
            var n = new NameGeneratorSerializer()
                .UsingProviderType(typeof(SyllableSet));

            var g2 = n.Deserialize(_syllableSetFile);

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

    }

}
