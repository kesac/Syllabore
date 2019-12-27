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

    }
}
