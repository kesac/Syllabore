using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Syllabore
{
    /// <summary>
    /// <p>
    /// Randomly generates names by constructing syllables and joining them together.
    /// It can also filter its output through a <see cref="IValidator"/> if one is specified.
    /// </p>
    /// <p>
    /// Use <c>Next()</c> to return names as strings and <c>NextName()</c>
    /// to return names as Name structs which gives you access to the syllable sequence.
    /// </p>
    /// </summary>
    public class NameGenerator : IGenerator, IMutator
    {
        private Random Random { get; set; }

        public SyllableProvider Provider { get; set; }
        public NameMutator Mutator { get; set; }
        public NameValidator Validator { get; set; }
        public int MinimumSyllables { get; set; }
        public int MaximumSyllables { get; set; }

        /// <summary>
        /// A number from 0 to 1 inclusive that represents the probablity
        /// that this NameGenerator's mutator will apply a mutation
        /// during name generation (during a call to Next() or NextName()).
        /// A value of 0 means a mutation can never occur and a value of 1
        /// means a mutation will always occur.
        /// </summary>
        public double MutationProbability { get; set; }

        /// <summary>
        /// Maximum attempts this generator will attempt to satisfy the
        /// NameValidator before it throws an Exception. This is used to protect
        /// against scenarios where a NameGenerator has been configured in such
        /// a way that it can't generate any name that would satisfy its own validator.
        /// </summary>
        public int MaximumRetries { get; set; }

        /// <summary>
        /// When there are no constructor arguments, the name generator is configured to
        /// use DefaultSyllableProvider, DefaultNameMutator, and no name validator.
        /// </summary>
        public NameGenerator() : this(new DefaultSyllableProvider(), new DefaultNameMutator(), null) { }

        public NameGenerator(SyllableProvider provider) : this(provider, new DefaultNameMutator(), null) { }

        public NameGenerator(SyllableProvider provider, NameMutator mutator) : this(provider, mutator, null) { }

        public NameGenerator(SyllableProvider provider, NameValidator validator) : this(provider, new DefaultNameMutator(), validator) { }

        public NameGenerator(SyllableProvider provider, NameMutator mutator, NameValidator validator)
        {
            this.UsingProvider(provider)
                .UsingMutator(mutator)
                .UsingValidator(validator)
                .LimitSyllableCount(2, 2)
                .LimitRetries(1000);

            this.Random = new Random();
        }

        /// <summary>
        /// Creates a new ConfigurableSyllableProvider for this NameGenerator.
        /// The new ConfigurableSyllableProvider replaces the old SyllableProvider if
        /// one is already defined.
        /// </summary>
        public NameGenerator UsingProvider(Func<SyllableProvider,SyllableProvider> configure)
        {
            this.Provider = configure(new SyllableProvider());
            return this;
        }

        /// <summary>
        /// Sets the specified ISyllableProvider as the new syllable provider for this NameGenerator.
        /// The old ISyllableProvider is replaced if one was previously defined.
        /// </summary>
        public NameGenerator UsingProvider(SyllableProvider provider)
        {
            this.Provider = provider ?? throw new ArgumentNullException("The specified ISyllableProvider is null.");
            return this;
        }

        public NameGenerator UsingValidator(Func<NameValidator, NameValidator> configure)
        {
            this.Validator = configure(new NameValidator());
            return this;
        }

        // Right now this is ok
        public NameGenerator UsingValidator(NameValidator validator)
        {
            this.Validator = validator;
            return this;
        }
        
        public NameGenerator UsingMutator(Func<NameMutator, NameMutator> configure)
        {
            this.Mutator = configure(new NameMutator());
            return this;
        }

        public NameGenerator UsingMutator(NameMutator mutator)
        {
            this.Mutator = mutator ?? throw new ArgumentNullException("The specified IMutator is null.");
            return this;
        }

        public NameGenerator LimitSyllableCount(int length)
        {
            return this.LimitSyllableCount(length, length);
        }

        public NameGenerator LimitSyllableCount(int min, int max)
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

        public NameGenerator LimitRetries(int limit)
        {

            if(limit < 1)
            {
                throw new ArgumentException("The number of maximum attempts to make must be one or greater.");
            }

            this.MaximumRetries = limit;

            return this;
        }

        public NameGenerator LimitMutationChance(double probability)
        {

            if (probability < 0 || probability > 1)
            {
                throw new ArgumentException("The mutation probability must be a number between 0 and 1 inclusive.");
            }

            this.MutationProbability = probability;

            return this;
        }

        /// <summary>
        /// Generates a random name. The syllable length of the generated name is determined by the properties <c>MinimumSyllables</c> and <c>MaximumSyllables</c>.
        /// If you need to access individual syllables, use NextName() instead.
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
        /// Generates a random name with the specified syllable length and returns as a string.
        /// If you need to access individual syllables, use NextName() instead.
        /// </summary>
        public string Next(int syllableLength)
        {
            return this.NextName(syllableLength).ToString();
        }

        /// <summary>
        /// Generates a random name and returns it as a Name struct. The syllable length of the generated name is determined by the properties <c>MinimumSyllables</c> and <c>MaximumSyllables</c>.
        /// </summary>
        public Name NextName()
        {
            if (this.MinimumSyllables < 1 || this.MaximumSyllables < 1 || this.MinimumSyllables > this.MaximumSyllables)
            {
                throw new InvalidOperationException("The value of property MinimumSyllables must be less than or equal to property MaximumSyllables. Both values must be postive integers.");
            }

            var syllableLength = this.Random.Next(this.MinimumSyllables, this.MaximumSyllables + 1);
            return this.NextName(syllableLength);
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

            var validNameGenerated = false;
            var totalAttempts = 0;
            var result = new Name();

            while (!validNameGenerated)
            {
                result.Syllables.Clear();

                for (int i = 0; i < syllableLength; i++)
                {
                    if (i == 0 && syllableLength > 1)
                    {
                        //result.Syllables[i] = this.Provider.NextStartingSyllable();
                        result.Syllables.Add(this.Provider.NextStartingSyllable());
                    }
                    else if (i == syllableLength - 1 && syllableLength > 1)
                    {
                        result.Syllables.Add(this.Provider.NextEndingSyllable());
                    }
                    else
                    {
                        result.Syllables.Add(this.Provider.NextSyllable());
                    }
                }

                if(this.Random.NextDouble() < this.MutationProbability)
                {
                    result =  this.Mutator.Mutate(result);
                }

                validNameGenerated = this.Validator != null ? this.Validator.IsValidName(result) : true;

                if (totalAttempts++ >= this.MaximumRetries && !validNameGenerated)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name. It may be configured in such a way that it cannot generate names that satisfy the specified NameValidator.");
                }
            }

            return result;
        }

        // Mutation will use this NameGenerator's mutator, but subject output to the validator (if there is one)
        public Name Mutate(Name sourceName)
        {

            if (sourceName.Syllables.Count < 1)
            {
                throw new ArgumentException("It's not possible to create variations on a name that has 0 syllables.");
            }

            var validNameGenerated = false;
            var totalAttempts = 0;
            Name result = null;

            while (!validNameGenerated)
            {
                result = this.Mutator.Mutate(sourceName);
                validNameGenerated = this.Validator != null ? this.Validator.IsValidName(result) : true;

                if (totalAttempts++ >= this.MaximumRetries && !validNameGenerated)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name variation through mutations. It may be configured in such a way that there does not exist any mutation that can satisfy the specified NameValidator."); ;
                }
            }

            return result ?? throw new InvalidOperationException("The NameGenerator has failed to produce a name variation through mutations.");
        }


    }
}
