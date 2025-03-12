﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Syllabore.Fluent;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameFilterTestsV3
    {
        private readonly NameGeneratorV3 _testNames;

        public NameFilterTestsV3()
        {
            // Example names: Sata, Resati, Tisara
            _testNames = new NameGeneratorV3("str", "aei");
        }

        [TestMethod]
        public void NameFilter_UsingNullFilter_GeneratesNames()
        {
            NameFilter sut = null;
            _testNames.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                var name = _testNames.Next();
                Assert.IsTrue(name.EndsWith("a", "e", "i"));
            }
        }

        [TestMethod]
        public void NameFilter_UsingEmptyFilter_GeneratesNames()
        {
            var sut = new NameFilter();
            _testNames.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                var name = _testNames.Next();
                Assert.IsTrue(name.EndsWith("a", "e", "i"));
            }
        }

        [TestMethod]
        public void NameFilter_PreventSpecificPrefix_FilterAffectsGeneration()
        {
            var sut = new NameFilter().DoNotAllowStart("s");
            _testNames.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                var name = _testNames.Next().ToLower();
                Assert.IsTrue(name.StartsWith("t") || name.StartsWith("r"));
            }
        }

        [TestMethod]
        public void NameFilter_PreventSpecificSuffix_FilterAffectsGeneration()
        {
            var sut = new NameFilter().DoNotAllowEnding("I");
            _testNames.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                var name = _testNames.Next().ToLower();
                Assert.IsFalse(name.EndsWith("i"));
            }
        }

        [TestMethod]
        [DataRow("s")]
        [DataRow("r")]
        [DataRow("e")]
        public void NameFilter_PreventSpecificSubstring_FilterAffectsGeneration(string substring)
        {
            var sut = new NameFilter().DoNotAllowSubstring(substring);
            _testNames.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                var name = _testNames.Next().ToLower();
                Assert.IsFalse(name.Contains(substring));
            }
        }

        [TestMethod]
        public void NameFilter_PreventSpecificPattern_FilterAffectsGeneration()
        {
            var names = new NameGeneratorV3()
                .Any(x => x
                    .First(x => x.Add("b").Cluster("cc"))
                    .Middle(x => x.Add("ae").Cluster("ee"))
                    .Last(x => x.Add("d").Cluster("ff")));

            // Disallow clusters in a filter
            var sut = new NameFilter()
                .DoNotAllowRegex(@"[eE]{2}")
                .DoNotAllowRegex(@"[cfCF]{2}");

            names.Filter(sut);

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(Regex.IsMatch(names.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod]
        public void NameFilter_UsingFluentMethod_FilterAffectsGeneration()
        {
            var sut = new NameGeneratorV3()
                    .Any(x => x
                        .First("bcdf").Chance(0.95)
                        .Middle("aei"))
                    .Filter("a", "^b");

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next().ToLower();
                Assert.IsFalse(name.ContainsAny("a"));
                Assert.IsFalse(name.StartsWith("b"));
            }
        }

        /// Move this test to NameGeneratorTestsV3 (it doesn't use name filters)
        [TestMethod]
        public void NameValidation_WhenPrefixConstraintNotSpecified_OutputReflectsConstraints()
        {
            var leading = new SyllableGeneratorV3().Add(SymbolPosition.Middle, "aei");
            var inner = leading.Copy().Add(SymbolPosition.First, "str");
            var trailing = inner.Copy();

            var sut = new NameGeneratorV3()
                .Set(SyllablePosition.Leading, leading)
                .Set(SyllablePosition.Inner, inner)
                .Set(SyllablePosition.Trailing, trailing);

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.StartsWith("A", "E", "I"));
            }
        }

    }
}
