using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Random Random { get; set; }

        /// <summary>
        /// The <see cref="SyllableGenerator">SyllableGenerators</see> used by this <see cref="NameGenerator"/>.
        /// </summary>
        public Dictionary<SyllablePosition, SyllableGenerator> SyllableGenerators { get; set; }

        /// <summary>
        /// The transformer used to modify generated names.
        /// </summary>
        public INameTransformer NameTransformer { get; set; }

        /// <summary>
        /// The chance that the name transformer will be applied to a generated name.
        /// This value should be between 0.0 and 1.0 and only has an effect if the 
        /// NameTransformer property is set.
        /// </summary>
        public double NameTransformerChance { get; set; }

        /// <summary>
        /// The filter used to control generated names.
        /// </summary>
        public NameFilter NameFilter { get; set; }

        /// <summary>
        /// The minimum number of syllables in generated names.
        /// </summary>
        public int MinimumSize { get; set; }

        /// <summary>
        /// The maximum number of syllables in generated names.
        /// </summary>
        public int MaximumSize { get; set; }

        /// <summary>
        /// If this generator has a filter, this is the maximum attempts that will be
        /// made to satisfy the filter before an InvalidOperationException is thrown.
        /// The default value is 1000.
        /// </summary>
        public int MaximumRetries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameGenerator"/> class.
        /// The default size range is 2 to 3 syllables.
        /// </summary>
        public NameGenerator()
        {
            SyllableGenerators = new Dictionary<SyllablePosition, SyllableGenerator>();
            Random = new Random();
            MinimumSize = 2;
            MaximumSize = 3;
            MaximumRetries = 1000;

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
        /// </summary>
        /// <param name="position">The position for the syllable generator.</param>
        /// <param name="generator">The syllable generator to add.</param>
        /// <returns>The current instance of <see cref="NameGenerator"/>.</returns>
        public NameGenerator SetSyllables(SyllablePosition position, SyllableGenerator generator)
        {
            if(position == SyllablePosition.Any)
            {
                SetSyllables(SyllablePosition.Leading, generator);
                SetSyllables(SyllablePosition.Inner, generator);
                SetSyllables(SyllablePosition.Trailing, generator);
            }
            else
            {
                SyllableGenerators[position] = generator;
            }

            return this;
        }

        /// <summary>
        /// Sets the name transformer to use when generating names.
        /// </summary>
        public NameGenerator SetTransform(INameTransformer transformer)
        {
            NameTransformer = transformer;
            NameTransformerChance = 1.0;
            return this;
        }

        /// <summary>
        /// Sets the name transformer to use when generating names.
        /// </summary>
        public NameGenerator SetTransform(double chance, INameTransformer transformer)
        {
            NameTransformer = transformer;
            NameTransformerChance = chance;
            return this;
        }

        /// <summary>
        /// Sets the name filter to use when generating names.
        /// </summary>
        public NameGenerator SetFilter(NameFilter filter)
        {
            NameFilter = filter;
            return this;
        }

        /// <summary>
        /// Sets both the minimum and maximum number of syllables to use per name.
        /// </summary>
        public NameGenerator SetSize(int size)
        {
            MinimumSize = size;
            MaximumSize = size;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum number of syllables to use per name.
        /// </summary>
        public NameGenerator SetSize(int minSize, int maxSize)
        {
            MinimumSize = minSize;
            MaximumSize = maxSize;
            return this;
        }

        /// <summary>
        /// Generates a name by concatenating syllables from different positions based on the specified size.
        /// </summary>
        public string Next()
        {
            var validNameGenerated = false;
            var totalAttempts = 0;
            Name result = null;

            while (!validNameGenerated)
            {
                result = GenerateName();
                
                if (NameTransformer != null && Random.NextDouble() < NameTransformerChance)
                {
                    result = NameTransformer.Apply(result);
                }

                if (NameFilter == null)
                {
                    validNameGenerated = true;
                }
                else
                {
                    validNameGenerated = NameFilter.IsValid(result.ToString());
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
            if (SyllableGenerators.Count == 0)
            {
                throw new InvalidOperationException("No syllable generators available to generate a name.");
            }

            if (MinimumSize <= 0 || MaximumSize <= 0 || MinimumSize > MaximumSize)
            {
                throw new InvalidOperationException("Invalid size range.");
            }

            int size = Random.Next(MinimumSize, MaximumSize + 1);
            var result = new Name();

            if (size == 1)
            {
                if (SyllableGenerators.ContainsKey(SyllablePosition.Leading))
                {
                    result.Append(SyllableGenerators[SyllablePosition.Leading].Next());
                }
                else
                {
                    throw new InvalidOperationException("No syllable generator available for the first position.");
                }
            }
            else if (size == 2)
            {
                if (SyllableGenerators.ContainsKey(SyllablePosition.Leading) && SyllableGenerators.ContainsKey(SyllablePosition.Trailing))
                {
                    result.Append(SyllableGenerators[SyllablePosition.Leading].Next());
                    result.Append(SyllableGenerators[SyllablePosition.Trailing].Next());
                }
                else
                {
                    throw new InvalidOperationException("No syllable generator available for the first or last position.");
                }
            }
            else
            {
                if (SyllableGenerators.ContainsKey(SyllablePosition.Leading) && SyllableGenerators.ContainsKey(SyllablePosition.Inner) && SyllableGenerators.ContainsKey(SyllablePosition.Trailing))
                {
                    result.Append(SyllableGenerators[SyllablePosition.Leading].Next());

                    for (int i = 1; i < size - 1; i++)
                    {
                        result.Append(SyllableGenerators[SyllablePosition.Inner].Next());
                    }

                    result.Append(SyllableGenerators[SyllablePosition.Trailing].Next());
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
