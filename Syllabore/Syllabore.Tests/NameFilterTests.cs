using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Syllabore.Fluent;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameFilterTests
    {
        private readonly NameGenerator _sut;
        public NameFilterTests()
        {
            _sut = new NameGenerator()
                    .UsingSyllables(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("str"));
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WithoutInstantiatingNameFilterExplicitly_OutputReflectsConstraints()
        {
            var sut = new NameGenerator()
                    .UsingSyllables(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("bcdf"))
                    .DoNotAllow("a", "^b", "c$", "e");

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next().ToLower();
                Assert.IsFalse(name.ContainsAny("a", "e"));
                Assert.IsFalse(name.StartsWith("b"));
                Assert.IsFalse(name.EndsWith("c"));
            }
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenPrefixConstraintNotSpecified_OutputReflectsConstraints()
        {
            var syllableGenerator = new SyllableGenerator()
                        .WithVowels("aei")
                        .WithConsonants("str")
                        .WithProbability(x => x.OfLeadingVowelsInStartingSyllable(1));

            var sut = new NameGenerator().UsingSyllables(syllableGenerator);

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.StartsWith("A", "E", "I"));
            }
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenPrefixConstraintSpecified_OutputReflectsConstraints()
        {
            var syllableGenerator = new SyllableGenerator()
                        .WithVowels("aei")
                        .WithConsonants("str")
                        .WithProbability(x => x.OfLeadingVowelsInStartingSyllable(1));

            var sut = new NameGenerator().UsingSyllables(syllableGenerator);
                    
            sut.UsingFilter(x => x.DoNotAllowStart("a")); // this method should be case insensitive too

            for (int i = 0; i < 1000; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.StartsWith("E", "I"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenNoSuffixConstraintSpecified_OutputReflectsConstraints()
        {
            for (int i = 0; i < 1000; i++)
            {
                var name = _sut.Next();
                Assert.IsTrue(name.EndsWith("a", "e", "i"));
            }
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenSuffixConstraintSpecified_OutputReflectsConstraints()
        {
            _sut.UsingFilter(x => x.DoNotAllowEnding("I")); // this method should be case insensitive too

            for (int i = 0; i < 1000; i++)
            {
                var name = _sut.Next();
                Assert.IsTrue(!name.EndsWith("I", "i"));
            }
        }

        [TestMethod, Timeout(10000)]
        [DataRow(new string[] { "a", "e", "i" })]
        public void NameValidation_WhenNonRegexConstraintSpecified_OutputReflectsConstraints(string[] characters)
        {
            for (int i = 0; i < 1000; i++)
            {
                var name = _sut.Next();
                Assert.IsTrue(name.ContainsAny(characters));
            }
        }

        [TestMethod, Timeout(10000)]
        [DataRow("e")]
        [DataRow("a")]
        [DataRow("q")]
        public void NameValidation_WhenNonRegexConstraintSpecified_WithDisallowedCharacter_OutputReflectsConstraints(string character)
        {
            _sut.UsingFilter(x => x.Add(character));
            
            for (int i = 0; i < 1000; i++)
            {
                var name = _sut.Next();
                Assert.IsTrue(!name.ContainsAny(character.ToLower(), character.ToUpper()));
            }
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified_OutputReflectsConstraints()
        {
            var sut = new NameGenerator()
                            .UsingSyllables(x => x
                                .WithVowels("a")
                                .WithVowelSequences("ee")
                                .WithLeadingConsonants("b")
                                .WithLeadingConsonantSequences("cc")
                                .WithTrailingConsonants("d")
                                .WithTrailingConsonantSequences("ff"))
                            .UsingFilter(x => x
                                .DoNotAllowRegex(@"[aeiouAEIOU]{2}") // This rule rejects names with vowel sequences
                                .DoNotAllowRegex(@"[^aeiouAEIOU]{2}")); // This rule rejects names with consonant sequences

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(Regex.IsMatch(sut.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified3_OutputReflectsConstraints()
        {
            var sut = new NameGenerator();
            sut.UsingSyllableCount(2, 3);

            var nameFilter = new NameFilter();
            nameFilter.DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z");
            nameFilter.DoNotAllowRegex("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row
            nameFilter.DoNotAllowRegex("(q[^u])"); // Q must always be followed by a u
            nameFilter.DoNotAllowRegex("([^tsao]w)"); // W must always be preceded with a t, s, a, or o
            nameFilter.DoNotAllowRegex("pn"); // Looks a little awkward

            Assert.IsFalse(nameFilter.IsValidName(new Name() { Syllables = new List<string>() { "qello" } }));
            Assert.IsTrue(nameFilter.IsValidName(new Name() { Syllables = new List<string>() { "quello" } }));

            Assert.IsFalse(nameFilter.IsValidName(new Name() { Syllables = new List<string>() { "lwas" } }));
            Assert.IsTrue(nameFilter.IsValidName(new Name() { Syllables = new List<string>() { "twas" } }));

            sut.UsingFilter(nameFilter);
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified2_OutputReflectsConstraints()
        {
            var provider = new DefaultSyllableGenerator();
            var filter = new NameFilter();
            filter.DoNotAllowRegex(@"[^aeiouAEIOU]{3,}"); // Rejects 3 or more consecutive consonants

            Assert.IsTrue(filter.IsValidName(new Name() { Syllables = new List<String>() { "bc" } }));
            Assert.IsFalse(filter.IsValidName(new Name() { Syllables = new List<String>() { "bcd" } }));
            Assert.IsFalse(filter.IsValidName(new Name() { Syllables = new List<String>() { "bcdf" } }));

            var sut = new NameGenerator();
            sut.UsingSyllables(provider);
            sut.UsingFilter(filter);

            for (int i = 0; i < 1000; i++)
            {
                var original = sut.Next().ToLower();
                var name = new StringBuilder(original);

                name.Replace("a", " ");
                name.Replace("e", " ");
                name.Replace("i", " ");
                name.Replace("o", " ");
                name.Replace("u", " ");
                
                var consonantGroups = name.ToString().Split(" ");

                int maxConsonantSequenceLength = 0;
                foreach(var sequence in consonantGroups)
                {
                    if(sequence.Length > maxConsonantSequenceLength)
                    {
                        maxConsonantSequenceLength = sequence.Length;
                    }
                }
                Assert.IsTrue(maxConsonantSequenceLength < 3);
            }
        }
    }
}
