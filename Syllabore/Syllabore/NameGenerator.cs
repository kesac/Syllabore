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
        private Random Random { get; set; }
        public ISyllableProvider Provider { get; private set; }
        public INameValidator Validator { get; private set; }
        public int MinimumSyllables { get; private set; }
        public int MaximumSyllables { get; private set; }

        /// <summary>
        /// Maximum attempts this generator will attempt to satisfy the
        /// NameValidator before it throws an Exception. This is used to protect
        /// against scenarios where a NameGenerator has been configured in such
        /// a way that it can't generate any name that would satisfy its own validator.
        /// </summary>
        public int MaximumRetries { get; set; }

        /// <summary>
        /// Convenience constructor to construct a name generator using StandaloneSyllableProvider.
        /// No NameValidator is configured when this constructor is used.
        /// </summary>
        public NameGenerator() : this(new StandaloneSyllableProvider()) { }

        /// <summary>
        /// Constructs a name generator using the specified SyllableProvider.
        /// No NameValidator is configured when this constructor is used; 
        /// </summary>
        public NameGenerator(ISyllableProvider provider)
        {
            this.SetProvider(provider);
            this.SetSyllableLength(2, 2);
            this.SetMaximumRetries(1000);
            this.Random = new Random();
        }

        /// <summary>
        /// Constructs a name generator using the specified syllable provider and name validator.
        /// </summary>
        public NameGenerator(ISyllableProvider provider, INameValidator validator) : this(provider)
        {
            this.SetValidator(validator);
        }

        public NameGenerator SetProvider(ISyllableProvider provider) 
        {
            this.Provider = provider ?? throw new ArgumentNullException("The specified ISyllableProvider is null.");
            return this;
        }

        public NameGenerator SetValidator(INameValidator validator)
        {
            this.Validator = validator ?? throw new ArgumentNullException("The specified INameValidator is null.");
            return this;
        }

        public NameGenerator SetSyllableLength(int length)
        {
            return this.SetSyllableLength(length, length);
        }

        public NameGenerator SetSyllableLength(int min, int max)
        {

            if(min < 1)
            {
                throw new ArgumentException("The minimum syllable length must be a positive number.");
            }
            else if (max < min)
            {
                throw new ArgumentException("The maximum syllable length must be equal or greater to the minimum syllable length.");
            }

            this.MinimumSyllables = min;
            this.MaximumSyllables = max;
            return this;
        }

        public NameGenerator SetMaximumRetries(int limit)
        {

            if(limit < 1)
            {
                throw new ArgumentException("The number of maximum attempts to make must be one or greater.");
            }

            this.MaximumRetries = limit;

            return this;
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
                
                if(totalAttempts++ >= this.MaximumRetries && !validNameGenerated)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name. It may be configured in such a way that it cannot generate names that satisfy the specified NameValidator.");
                }
            }

            return new Name(syllables.ToArray());
        }



    }
}
