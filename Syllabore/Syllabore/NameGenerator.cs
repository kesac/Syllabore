using Archigen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Syllabore
{
    /// <summary>
    /// Generates names by constructing syllables and joining them together.
    /// </summary>
    public class NameGenerator : IGenerator<string>, IRandomizable
    {
        /// <summary>
        /// The instance of <see cref="System.Random"/> used to simulate randomness.
        /// </summary>
        [JsonIgnore]
        public Random Random { get; set; }

        /// <summary>
        /// The <see cref="SyllableGenerator">SyllableGenerators</see> used by this <see cref="NameGenerator"/>.
        /// </summary>
        public Dictionary<SyllablePosition, ISyllableGenerator> SyllableGenerators { get; set; }

        /// <summary>
        /// The transformer used to modify generated names. Can be null if no transform is being used.
        /// </summary>
        public INameTransformer NameTransformer { get; set; }

        /// <summary>
        /// The filter used to control generated names. Can be null if no filter is being used.
        /// </summary>
        public INameFilter NameFilter { get; set; }

        /// <summary>
        /// The minimum number of syllables in generated names. The default minimum size is 2 syllables.
        /// </summary>
        public int MinimumSize { get; set; }

        /// <summary>
        /// The maximum number of syllables in generated names. The default maximum size is 3 syllables.
        /// </summary>
        public int MaximumSize { get; set; }

        /// <summary>
        /// If this generator has a filter, this is the maximum attempts that will be
        /// made to satisfy the filter before an InvalidOperationException is thrown.
        /// The default maximum retry count is 1000.
        /// </summary>
        public int MaximumRetries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameGenerator"/> class
        /// with no symbols, no transformer, and no filter.
        /// </summary>
        public NameGenerator()
        {
            this.SyllableGenerators = new Dictionary<SyllablePosition, ISyllableGenerator>();
            this.Random = new Random();
            this.MinimumSize = 2;
            this.MaximumSize = 3;
            this.MaximumRetries = 1000;

            // The NameFilter property is intentionally left null
            // The INameTransformer property is intentionally left null
        }

        /// <summary>
        /// Initializes a new <see cref="NameGenerator">NameGenerator</see> 
        /// with specified symbol pools for the first and middle symbol positions of a syllable.
        /// Each character in the provided strings is considered a separate symbol.
        /// The generated syllables will be used for all positions of a name.
        /// </summary>
        public NameGenerator(string firstSymbols, string middleSymbols) : this()
        {
            var syllableGenerator = new SyllableGenerator()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Middle, middleSymbols);

            this.SetSyllables(SyllablePosition.Any, syllableGenerator);
        }

        /// <summary>
        /// Initializes a new <see cref="NameGenerator">NameGenerator</see> 
        /// with specified symbol pools for the first, middle, and last symbol positions of a syllable.
        /// Each character in the provided strings is considered a separate symbol.
        /// The generated syllables will be used for all positions of a name.
        /// </summary>
        public NameGenerator(string firstSymbols, string middleSymbols, string lastSymbols) : this()
        {
            var syllableGenerator = new SyllableGenerator()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Middle, middleSymbols)
                .Add(SymbolPosition.Last, lastSymbols);

            this.SetSyllables(SyllablePosition.Any, syllableGenerator);
        }

        /// <summary>
        /// Sets the <see cref="SyllableGenerator"/> for the specified position.
        /// Returns this instance of <see cref="NameGenerator"/> for chaining.
        /// </summary>
        /// <param name="position">The position for the syllable generator.</param>
        /// <param name="generator">The syllable generator to add.</param>
        /// <returns>The current instance of <see cref="NameGenerator"/>.</returns>
        public NameGenerator SetSyllables(SyllablePosition position, ISyllableGenerator generator)
        {
            if(position == SyllablePosition.Any)
            {
                SetSyllables(SyllablePosition.Starting, generator);
                SetSyllables(SyllablePosition.Inner, generator);
                SetSyllables(SyllablePosition.Ending, generator);
            }
            else
            {
                this.SyllableGenerators[position] = generator;
            }

            return this;
        }

        /// <summary>
        /// Sets the name transformer to use when generating names.
        /// Returns this instance of <see cref="NameGenerator"/> for chaining.
        /// </summary>
        public NameGenerator SetTransform(INameTransformer transformer)
        {
            this.NameTransformer = transformer;
            return this;
        }

        /// <summary>
        /// Sets the name filter to use when generating names.
        /// Returns this instance of <see cref="NameGenerator"/> for chaining.
        /// </summary>
        public NameGenerator SetFilter(INameFilter filter)
        {
            this.NameFilter = filter;
            return this;
        }

        /// <summary>
        /// Sets both the minimum and maximum number of syllables to use per name.
        /// Returns this instance of <see cref="NameGenerator"/> for chaining.
        /// </summary>
        public NameGenerator SetSize(int size)
        {
            this.MinimumSize = size;
            this.MaximumSize = size;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum number of syllables to use per name.
        /// Returns this instance of <see cref="NameGenerator"/> for chaining.
        /// </summary>
        public NameGenerator SetSize(int minSize, int maxSize)
        {
            this.MinimumSize = minSize;
            this.MaximumSize = maxSize;
            return this;
        }

        /// <summary>
        /// Generates a name and returns it as a string.
        /// </summary>
        public string Next()
        {
            var validNameGenerated = false;
            var totalAttempts = 0;
            Name result = null;

            while (!validNameGenerated)
            {
                result = GenerateName();

                // Note that a transform is a potential action
                // and if its Chance property is below 100%, nothing
                // may change
                if (NameTransformer != null)
                {
                    result = NameTransformer.Apply(result);
                }

                if (NameFilter == null)
                {
                    validNameGenerated = true;
                }
                else
                {
                    validNameGenerated = NameFilter.IsValid(result);
                }

                if (!validNameGenerated && ++totalAttempts >= MaximumRetries)
                {
                    throw new InvalidOperationException("Could not generate a valid name after " + MaximumRetries + " attempts.");
                }
            }

            return result.ToString();
        }

        private Name GenerateName()
        {
            if (this.SyllableGenerators.Count == 0)
            {
                throw new InvalidOperationException("No syllable generators available to generate a name.");
            }

            if (this.MinimumSize <= 0 || this.MaximumSize <= 0 || this.MinimumSize > this.MaximumSize)
            {
                throw new InvalidOperationException("Invalid size range.");
            }

            int size = this.Random.Next(this.MinimumSize, this.MaximumSize + 1);
            var result = new Name();

            if (size == 1)
            {
                if (this.SyllableGenerators.ContainsKey(SyllablePosition.Starting))
                {
                    result.Append(this.SyllableGenerators[SyllablePosition.Starting].Next());
                }
                else
                {
                    throw new InvalidOperationException("No syllable generator available for the starting position.");
                }
            }
            else if (size == 2)
            {
                if (this.SyllableGenerators.ContainsKey(SyllablePosition.Starting) 
                    && this.SyllableGenerators.ContainsKey(SyllablePosition.Ending))
                {
                    result.Append(this.SyllableGenerators[SyllablePosition.Starting].Next());
                    result.Append(this.SyllableGenerators[SyllablePosition.Ending].Next());
                }
                else
                {
                    throw new InvalidOperationException("No syllable generator available for the starting or ending position.");
                }
            }
            else
            {
                if (this.SyllableGenerators.ContainsKey(SyllablePosition.Starting) 
                    && this.SyllableGenerators.ContainsKey(SyllablePosition.Inner) 
                    && SyllableGenerators.ContainsKey(SyllablePosition.Ending))
                {
                    result.Append(this.SyllableGenerators[SyllablePosition.Starting].Next());

                    for (int i = 1; i < size - 1; i++)
                    {
                        result.Append(this.SyllableGenerators[SyllablePosition.Inner].Next());
                    }

                    result.Append(this.SyllableGenerators[SyllablePosition.Ending].Next());
                }
                else
                {
                    throw new InvalidOperationException("Not enough SyllableGenerators to create a name of the desired size.");
                }
            }

            // Capitalize the first character of the generated name
            return result;
        }
    }
}
