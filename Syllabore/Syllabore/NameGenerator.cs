﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Randomly generates names by constructing syllables and joining them together.
    /// Optionally, it can also filter its output through a <c>INameValidator</c>.
    /// </summary>
    public class NameGenerator
    {
        private ISyllableProvider Provider { get; set; }
        private INameValidator Validator { get; set; }
        private Random Random { get; set; }
        public int MinimumSyllables { get; set; }
        public int MaximumSyllables { get; set; }

        /// <summary>
        /// Constructs a name generator with no validator using the specified syllable provider.
        /// </summary>
        public NameGenerator(ISyllableProvider provider)
        {
            this.Provider = provider;
            this.MinimumSyllables = 2;
            this.MaximumSyllables = 2;
            this.Random = new Random();
        }

        /// <summary>
        /// Constructs a name generator using the specified syllable provider and name validator.
        /// </summary>
        public NameGenerator(ISyllableProvider provider, INameValidator validator) : this(provider)
        {
            if (validator != null)
            {
                this.Validator = validator;
            }
        }


        /// <summary>
        /// Generates a random name. The syllable length of the generated name is determined by the properties <c>MinimumSyllables</c> and <c>MaximumSyllables</c>.
        /// </summary>
        public string Next()
        {
            var syllableLength = this.Random.Next(this.MinimumSyllables, this.MaximumSyllables + 1);
            return this.Next(syllableLength);
        }

        /// <summary>
        /// Generates a random name with the specified syllable length.
        /// </summary>
        public string Next(int syllableLength)
        {
            var output = new StringBuilder();
            var validNameGenerated = false;

            while (!validNameGenerated)
            {
                output.Clear();
                for (int i = 0; i < syllableLength; i++)
                {
                    if (i == 0 && syllableLength > 1)
                    {
                        output.Append(Provider.NextStartingSyllable());
                    }
                    else if (i == syllableLength - 1 && syllableLength > 1)
                    {
                        output.Append(Provider.NextEndingSyllable());
                    }
                    else
                    {
                        output.Append(Provider.NextSyllable());
                    }
                }

                if (this.Validator != null)
                {
                    validNameGenerated = this.Validator.IsValidName(output.ToString());
                }
                else
                {
                    validNameGenerated = true;
                }
            }

            return output.ToString().Substring(0, 1) + output.ToString().Substring(1).ToLower();
        }



    }
}
