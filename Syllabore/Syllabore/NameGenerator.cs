using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <p>
    /// Randomly generates names by constructing syllables and joining them together.
    /// Optionally, it can also filter its output through a <c>INameValidator</c>.
    /// </p>
    /// <p>
    /// Use <c>Next()</c> to return names as strings and <c>NextName()</c>
    /// to return names as Name structs which gives you access to the syllable sequence.
    /// </p>
    /// </summary>
    public class NameGenerator
    {
        private ISyllableProvider Provider { get; set; }
        private INameValidator Validator { get; set; }
        private Random Random { get; set; }
        public int MinimumSyllables { get; set; }
        public int MaximumSyllables { get; set; }

        /// <summary>
        /// Maximum attempts this generator will attempt to satisfy the
        /// NameValidator before it throws an Exception. This is used to protect
        /// against scenarios where a NameGenerator has been configured in such
        /// a way that it can't generate any name that would satisfy the validator.
        /// </summary>
        public int MaximumAttempts { get; set; }

        /// <summary>
        /// Constructs a name generator with no validator using the specified syllable provider.
        /// </summary>
        public NameGenerator(ISyllableProvider provider)
        {
            this.Provider = provider ?? throw new ArgumentNullException("The specified ISyllableProvider is null.");
            this.MinimumSyllables = 2;
            this.MaximumSyllables = 2;
            this.MaximumAttempts = 1000;
            this.Random = new Random();
        }

        /// <summary>
        /// Constructs a name generator using the specified syllable provider and name validator.
        /// </summary>
        public NameGenerator(ISyllableProvider provider, INameValidator validator) : this(provider)
        {
            this.Validator = validator ?? throw new ArgumentNullException("The specified INameValidator is null.");
        }

        /// <summary>
        /// Generates a random name. The syllable length of the generated name is determined by the properties <c>MinimumSyllables</c> and <c>MaximumSyllables</c>.
        /// </summary>
        public string Next()
        {
            // We validate the minimum and maximum syllable lengths in this method instead of the property setters because per Microsoft:
            // "...exceptions resulting from invalid state should be postponed until the interraleted properties are actually used together..."
            // (https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/property?redirectedfrom=MSDN)
            if (this.MinimumSyllables < 1 || this.MaximumSyllables < 1 || this.MinimumSyllables > this.MaximumSyllables)
            {
                throw new InvalidOperationException("The value of property MinimumSyllables must be less than or equal to property MaximumSyllables. Both values must be postive integers.");
            }

            var syllableLength = this.Random.Next(this.MinimumSyllables, this.MaximumSyllables + 1);
            return this.Next(syllableLength);
        }

        /// <summary>
        /// Generates a random name and returns it as a Name struct. The syllable length of the generated name is determined by the properties <c>MinimumSyllables</c> and <c>MaximumSyllables</c>.
        /// </summary>
        public Name NextName()
        {
            if(this.MinimumSyllables < 1 || this.MaximumSyllables < 1 || this.MinimumSyllables > this.MaximumSyllables)
            {
                throw new InvalidOperationException("The value of property MinimumSyllables must be less than or equal to property MaximumSyllables. Both values must be postive integers.");
            }

            var syllableLength = this.Random.Next(this.MinimumSyllables, this.MaximumSyllables + 1);
            return this.NextName(syllableLength);
        }

        /// <summary>
        /// Generates a random name with the specified syllable length.
        /// </summary>
        public string Next(int syllableLength)
        {
            return this.NextName(syllableLength).ToString();
        }

        /// <summary>
        /// Generates a random name with the specified syllable length and returns it as a Name struct.
        /// </summary>
        public Name NextName(int syllableLength)
        {

            if(syllableLength < 1)
            {
                throw new ArgumentException("The desired syllable length must be a positive value.");
            }

            var syllables = new List<string>();
            var validNameGenerated = false;
            var totalAttempts = 0;

            while (!validNameGenerated)
            {
                syllables.Clear();
                for (int i = 0; i < syllableLength; i++)
                {
                    if (i == 0 && syllableLength > 1)
                    {
                        syllables.Add(Provider.NextStartingSyllable());
                    }
                    else if (i == syllableLength - 1 && syllableLength > 1)
                    {
                        syllables.Add(Provider.NextEndingSyllable());
                    }
                    else
                    {
                        syllables.Add(Provider.NextSyllable());
                    }
                }

                if (this.Validator != null)
                {
                    validNameGenerated = this.Validator.IsValidName(syllables.ToString());
                }
                else
                {
                    validNameGenerated = true;
                }
                
                if(totalAttempts++ >= this.MaximumAttempts)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name. It may be configured in such a way that it cannot generate names that satisfy the specified NameValidator.");
                }
            }

            return new Name(syllables.ToArray());
        }



    }
}
