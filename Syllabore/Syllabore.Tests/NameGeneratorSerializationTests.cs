using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Syllabore.Fluent;

namespace Syllabore.Tests
{
    
    [TestClass]
    public class NameGeneratorSerializationTests
    {
        
        private readonly NameGenerator _defaultNameGenerator;
        private readonly string _defaultFile = "name-generator.json";
        private readonly string _syllableSetFile = "name-generator-with-syllable-set.json";

        public NameGeneratorSerializationTests()
        {
            _defaultNameGenerator = new NameGenerator()
                .Any(x => x
                    .First(x => x.Add("bcd").Cluster("dr"))
                    .Middle(x => x.Add("aieou").Cluster("ey"))
                    .Last(x => x.Add("trs").Cluster("mn")))
                .Filter("zzzy", "abcd")
                .Transform(new TransformSet()
                    .Chance(0.5)
                    .RandomlySelect(2)
                    .Add(x => x.Append("tar"))
                    .Add(x => x.Insert(0, "arc"))
                    .Add(x => x.Replace(0, "neo")))
                .SetSize(3);
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
            var t1 = _defaultNameGenerator.NameTransformer as TransformSet; // The default type
            var t2 = g2.NameTransformer as TransformSet;

            Assert.AreEqual(t1.RandomSelectionCount, t2.RandomSelectionCount);

        }
        
        [TestMethod]
        public void Serializer_DefaultType4_DeserializationPreservesSyllableGeneratorGraphemes()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);

            var p1 = _defaultNameGenerator.SyllableGenerators.First().Value as SyllableGenerator;  // The default type
            var p2 = g2.SyllableGenerators.First().Value as SyllableGenerator;

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

            var p1 = _defaultNameGenerator.SyllableGenerators.First().Value as SyllableGenerator;  // The default type
            var p2 = g2.SyllableGenerators.First().Value as SyllableGenerator;

            var syllableGeneratorType = typeof(SyllableGenerator);
            var syllableGeneratorProperties = syllableGeneratorType.GetProperties();

            // Comparing usage flags like  LeadingConsonantsAllowed, VowelSequencesAllowed, etc.
            foreach (var property in syllableGeneratorProperties.Where(x => x.PropertyType == typeof(bool)))
            {
                Assert.IsTrue(property.GetValue(p1).Equals(property.GetValue(p2)));
            }

        }
        

        
        [TestMethod]
        public void Serializer_DefaultType7_DeserializationPreservesFilters()
        {
            var sut = new NameGeneratorSerializer();
            var g2 = sut.Deserialize(_defaultFile);
            var f = (NameFilter)_defaultNameGenerator.NameFilter;
            var f2 = (NameFilter)g2.NameFilter;

            Assert.IsTrue(f.Constraints.UnorderedListEquals(f2.Constraints));

        }

        [TestMethod]
        public void Serializer_NonDefaultType1_SerializationSucceeds()
        {

            var syllables = new SyllableSet("ko", "ro");
            var g = new NameGenerator().SetSyllables(SyllablePosition.Any, syllables);
            var sut = new NameGeneratorSerializer();

            sut.Serialize(g, _syllableSetFile);
            Assert.IsTrue(File.Exists(_syllableSetFile));
        }

        [TestMethod]
        public void Serializer_NonDefaultType2_DeserializationSucceeds()
        {
            var sut = new NameGeneratorSerializer();

            var g2 = sut.Deserialize(_syllableSetFile);

            Assert.IsTrue(g2.SyllableGenerators.Count > 0);
            Assert.IsTrue(g2.SyllableGenerators.First().Value is SyllableSet);

            var s = g2.SyllableGenerators.First().Value as SyllableSet;

            for (int i = 0; i < 50; i++)
            {
                g2.Next();
            }

            Assert.IsTrue(s.PossibleSyllables.Count == 2);

        }

  
    }

}
