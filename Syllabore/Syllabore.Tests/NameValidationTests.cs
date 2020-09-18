using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameValidationTests
    {

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified_OutputReflectsConstraints()
        {
            var generator = new NameGenerator()
                .UsingProvider(x => x
                    .WithVowels("a")
                    .WithVowelSequences("ee")
                    .WithLeadingConsonants("b")
                    .WithLeadingConsonantSequences("cc")
                    .WithTrailingConsonants("d")
                    .WithTrailingConsonantSequences("ff")
                    .AllowVowelSequences() // These flags are true by default; we explicitly set them for clarity to future readers
                    .AllowLeadingConsonantSequences()
                    .AllowTrailingConsonantSequences())
                .UsingValidator(x => x
                    .Invalidate(@"[aeiouAEIOU]{2}") // This rule rejects names with vowel sequences
                    .Invalidate(@"[^aeiouAEIOU]{2}")); // This rule rejects names with consonant sequences

            for (int i = 1000; i < 1; i++)
            {
                Assert.IsFalse(Regex.IsMatch(generator.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified2_OutputReflectsConstraints()
        {

            var provider = new DefaultSyllableProvider();
            var validator = new ConfigurableNameValidator();
            validator.Invalidate(@"[^aeiouAEIOU]{3,}"); // Rejects 3 or more consecutive consonants

            Assert.IsTrue(validator.IsValidName("bc"));
            Assert.IsFalse(validator.IsValidName("bcd"));
            Assert.IsFalse(validator.IsValidName("bcdf"));

            var generator = new NameGenerator();
            generator.UsingProvider(provider);
            generator.UsingValidator(validator);

            for (int i = 1000; i < 1; i++)
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
