using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameFilterTests
    {

        /* Create a unit test that calls methods that start with "DoNot" and verifies that the name is not valid. */
        [TestMethod, Timeout(10000)]
        public void NameValidation_WithoutInstantiatingNameFilterExplicitly_OutputReflectsConstraints()
        {
            // Purposely using depcreated method UsingProvider()
            var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("bcdf"))
                    .DoNotAllow("a")
                    .DoNotAllow("^b")
                    .DoNotAllow("c$")
                    .DoNotAllow("e");

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next().ToLower();
                Assert.IsFalse(name.ContainsAny("a", "e"));
                Assert.IsFalse(name.StartsWith("b"));
                Assert.IsFalse(name.EndsWith("c"));
            }

        }



            [TestMethod, Timeout(10000)]
        public void NameValidation_WhenPrefixConstraintSpecified_OutputReflectsConstraints()
        {
            var p = new SyllableProvider()
                        .WithVowels("aei")
                        .WithConsonants("str")
                        .WithProbability(x => x.StartingSyllable.LeadingVowelExists(1));

            var g = new NameGenerator().UsingSyllables(p);
                    
            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.StartsWith("A","E","I"));
            }

            g.UsingFilter(x => x.DoNotAllowStart("a")); // this method should be case insensitive too

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.StartsWith("E", "I"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenSuffixConstraintSpecified_OutputReflectsConstraints()
        {
            var g = new NameGenerator()
                    .UsingSyllables(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("str"));

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.EndsWith("a", "e", "i"));
            }

            g.UsingFilter(x => x.DoNotAllowEnding("I")); // this method should be case insensitive too

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.EndsWith("a", "e"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenNonRegexConstraintSpecified_OutputReflectsConstraints()
        {
            var g = new NameGenerator()
                    .UsingSyllables(x => x
                        .WithVowels("aei")
                        .WithLeadingConsonants("str"));

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.ContainsAny("a", "e", "i"));
            }

            g.UsingFilter(x => x.DoNotAllow("e")); // this method should be case insensitive too

            for (int i = 0; i < 1000; i++)
            {
                var name = g.Next();
                Assert.IsFalse(name.ContainsAny("E","e"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified_OutputReflectsConstraints()
        {
            var generator = new NameGenerator()
                            .UsingSyllables(x => x
                                .WithVowels("a")
                                .WithVowelSequences("ee")
                                .WithLeadingConsonants("b")
                                .WithLeadingConsonantSequences("cc")
                                .WithTrailingConsonants("d")
                                .WithTrailingConsonantSequences("ff"))
                            .UsingFilter(x => x
                                .DoNotAllowPattern(@"[aeiouAEIOU]{2}") // This rule rejects names with vowel sequences
                                .DoNotAllowPattern(@"[^aeiouAEIOU]{2}")); // This rule rejects names with consonant sequences

            for (int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(Regex.IsMatch(generator.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified3_OutputReflectsConstraints()
        {
            var g = new NameGenerator();
            g.UsingSyllableCount(2, 3);

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "h", "j", "q", "v", "w", "z");
            f.DoNotAllowPattern("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            f.DoNotAllowPattern("(q[^u])"); // Q must always be followed by a u
            f.DoNotAllowPattern("([^tsao]w)"); // W must always be preceded with a t, s, a, or o
            f.DoNotAllow("pn"); // Looks a little awkward

            Assert.IsFalse(f.IsValidName(new Name() { Syllables = new List<string>() { "qello" } }));
            Assert.IsTrue(f.IsValidName(new Name() { Syllables = new List<string>() { "quello" } }));

            Assert.IsFalse(f.IsValidName(new Name() { Syllables = new List<string>() { "lwas" } }));
            Assert.IsTrue(f.IsValidName(new Name() { Syllables = new List<string>() { "twas" } }));

            g.UsingFilter(f);
        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified2_OutputReflectsConstraints()
        {

            var provider = new DefaultSyllableProvider();
            var filter = new NameFilter();
            filter.DoNotAllowPattern(@"[^aeiouAEIOU]{3,}"); // Rejects 3 or more consecutive consonants

            Assert.IsTrue(filter.IsValidName(new Name() { Syllables = new List<String>() { "bc" } }));
            Assert.IsFalse(filter.IsValidName(new Name() { Syllables = new List<String>() { "bcd" } }));
            Assert.IsFalse(filter.IsValidName(new Name() { Syllables = new List<String>() { "bcdf" } }));

            var generator = new NameGenerator();
            generator.UsingSyllables(provider);
            generator.UsingFilter(filter);

            for (int i = 0; i < 1000; i++)
            {
                var original = generator.Next();
                var name = new StringBuilder(original.ToLower());

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
