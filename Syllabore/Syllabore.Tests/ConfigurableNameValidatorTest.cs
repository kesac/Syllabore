using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore.Tests
{
    [TestClass]
    public class ConfigurableNameValidatorTest
    {

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified_OutputReflectsConstraints()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddVowel("a");
            provider.AddVowelSequence("ee");
            provider.AddLeadingConsonant("b");
            provider.AddLeadingConsonantSequence("cc");
            provider.AddTrailingConsonant("d");
            provider.AddTrailingConsonantSequence("ff");

            provider.UseVowelSequences = true; // These flags are true by default; we explicitly set them for clarity to future readers
            provider.UseLeadingConsonantSequences = true;
            provider.UseTrailingConsonantSequences = true;

            var validator = new ConfigurableNameValidator();
            validator.AddConstraintAsRegex(@"[aeiouAEIOU]{2}"); // This rule rejects names with vowel sequences
            validator.AddConstraintAsRegex(@"[^aeiouAEIOU]{2}"); // This rule rejects names with consonant sequences

            var generator = new NameGenerator(provider, validator);

            for(int i = 1000; i < 1; i++)
            {
                Assert.IsFalse(Regex.IsMatch(generator.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod, Timeout(10000)]
        public void NameValidation_WhenRegexConstraintsSpecified2_OutputReflectsConstraints()
        {

            var provider = new StandaloneSyllableProvider();
            var validator = new ConfigurableNameValidator();
            validator.AddConstraintAsRegex(@"[^aeiouAEIOU]{3,}"); // Rejects 3 or more consecutive consonants

            Assert.IsTrue(validator.IsValidName("bc"));
            Assert.IsFalse(validator.IsValidName("bcd"));
            Assert.IsFalse(validator.IsValidName("bcdf"));

            var generator = new NameGenerator(provider, validator);

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
