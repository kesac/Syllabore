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
    /// It can also filter its output through a <see cref="INameFilter"/> if one is specified.
    /// </p>
    /// <p>
    /// Use <c>Next()</c> to return names as strings and <c>NextName()</c>
    /// to return names as Name structs which gives you access to the syllable sequence.
    /// </p>
    /// </summary>
    public class NameGenerator : INameGenerator
    {

        private const double DefaultTransformChance = 1.0;

        private Random Random { get; set; }

        public SyllableProvider Provider { get; set; }
        public NameTransformer Modifier { get; set; }
        public NameFilter Filter { get; set; }
        public int MinimumSyllables { get; set; }
        public int MaximumSyllables { get; set; }

        /// <summary>
        /// Maximum attempts this generator will attempt to satisfy the
        /// NameFilter before it throws an Exception. This is used to protect
        /// against scenarios where a NameGenerator has been configured in such
        /// a way that it can't generate any name that would satisfy its own filter.
        /// </summary>
        public int MaximumRetries { get; set; }

        /// <summary>
        /// When there are no constructor arguments, the name generator is configured to
        /// use DefaultSyllableProvider, DefaultNameMutator, and no name filter.
        /// </summary>
        public NameGenerator() : this(new DefaultSyllableProvider(), new DefaultNameTransformer(), null) { }

        public NameGenerator(SyllableProvider provider) : this(provider, new DefaultNameTransformer(), null) { }

        public NameGenerator(SyllableProvider provider, NameTransformer mutator) : this(provider, mutator, null) { }

        public NameGenerator(SyllableProvider provider, NameFilter filter) : this(provider, new DefaultNameTransformer(), filter) { }

        public NameGenerator(SyllableProvider provider, NameTransformer mutator, NameFilter filter)
        {
            this.UsingProvider(provider)
                .UsingTransformer(mutator)
                .UsingFilter(filter)
                .UsingSyllableCount(2, 2)
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

        public NameGenerator UsingFilter(Func<NameFilter, NameFilter> configure)
        {
            this.Filter = configure(new NameFilter());
            return this;
        }

        // Right now this is ok
        public NameGenerator UsingFilter(NameFilter filter)
        {
            this.Filter = filter;
            return this;
        }
        
        public NameGenerator UsingTransformer(Func<NameTransformer, NameTransformer> configure)
        {
            this.Modifier = configure(new NameTransformer());

            
            if (!this.Modifier.TransformChance.HasValue)
            {
                this.Modifier.TransformChance = DefaultTransformChance;
            }
            

            return this;
        }

        public NameGenerator UsingTransformer(NameTransformer mutator)
        {
            this.Modifier = mutator ?? throw new ArgumentNullException("The specified IMutator is null.");
            return this;
        }

        public NameGenerator UsingSyllableCount(int length)
        {
            return this.UsingSyllableCount(length, length);
        }

        public NameGenerator UsingSyllableCount(int min, int max)
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

                if(this.Modifier.TransformChance.HasValue && this.Random.NextDouble() < this.Modifier.TransformChance)
                {
                    result = this.Modifier.Transform(result);
                }

                validNameGenerated = this.Filter != null ? this.Filter.IsValidName(result) : true;

                if (totalAttempts++ >= this.MaximumRetries && !validNameGenerated)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name. It may be configured in such a way that it cannot generate names that satisfy the specified NameFilter.");
                }
            }

            return result;
        }

        // If this gets uncommented, remember to add the ITransformer interface again
        //
        // Mutation will use this NameGenerator's mutator, but subject output to the filter (if there is one).
        // Using transforming a name this way will ignore the transform chance.
        /* 
        public Name Transform(Name sourceName)
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
                result = this.Modifier.Transform(sourceName);
                validNameGenerated = this.Filter != null ? this.Filter.IsValidName(result) : true;

                if (totalAttempts++ >= this.MaximumRetries && !validNameGenerated)
                {
                    throw new InvalidOperationException("This NameGenerator has run out of attempts generating a valid name variation through mutations. It may be configured in such a way that there does not exist any mutation that can satisfy the specified NameFilter."); ;
                }
            }

            return result ?? throw new InvalidOperationException("The NameGenerator has failed to produce a name variation through mutations.");
        }
        /**/


    }
}
