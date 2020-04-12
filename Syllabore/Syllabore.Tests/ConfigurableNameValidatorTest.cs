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

        [TestMethod]
        public void NameValidation_WhenRegexConstraintsSpecified_OutputReflectsConstraints()
        {
            var provider = new ConfigurableSyllableProvider();
            provider.AddVowel("a");
            provider.AddVowelSequence("ee");
            provider.AddStartingConsonant("b");
            provider.AddStartingConsonantSequence("cc");
            provider.AddEndingConsonant("d");
            provider.AddEndingConsonantSequence("ff");

            provider.UseVowelSequences = true; // These flags are true by default; we explicitly set them for clarity to future readers
            provider.UseStartingConsonantSequences = true;
            provider.UseEndingConsonantSequences = true;

            var validator = new ConfigurableNameValidator();
            validator.AddConstraintAsRegex("([a-zA-Z])\\1"); // This rule rejects names with vowel sequences or consonant sequences

            var generator = new NameGenerator(provider, validator);

            for(int i = 0; i < 1000; i++)
            {
                Assert.IsFalse(Regex.IsMatch(generator.Next(), "(ee|cc|ff)"));
            }

        }

        [TestMethod]
        public void NameValidation_WhenRegexConstraintsSpecified2_OutputReflectsConstraints()
        {

            var provider = new StandaloneSyllableProvider();
            var validator = new ConfigurableNameValidator();
            validator.AddConstraintAsRegex("^.*([^aeiouAEIOU]{3,}).*$"); // Rejects 3 or more consecutive consonants

            Assert.IsTrue(validator.IsValidName("bc"));
            Assert.IsFalse(validator.IsValidName("bcd"));
            Assert.IsFalse(validator.IsValidName("bcdf"));

            var generator = new NameGenerator(provider, validator);


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
