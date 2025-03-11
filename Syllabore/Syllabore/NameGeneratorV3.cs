using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates names by constructing syllables and joining them together.
    /// </summary>
    public class NameGeneratorV3 : IGenerator<string>, IRandomizable
    {
        /// <summary>
        /// The instance of <see cref="System.Random"/> used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The <see cref="SyllableGeneratorV3">SyllableGenerators</see> used by this <see cref="NameGeneratorV3"/>.
        /// </summary>
        public Dictionary<SyllablePosition, SyllableGeneratorV3> SyllableGenerators { get; set; }

        /// <summary>
        /// The minimum number of syllables in generated names.
        /// </summary>
        public int MinimumSize { get; set; }

        /// <summary>
        /// The maximum number of syllables in generated names.
        /// </summary>
        public int MaximumSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameGeneratorV3"/> class.
        /// </summary>
        public NameGeneratorV3()
        {
            SyllableGenerators = new Dictionary<SyllablePosition, SyllableGeneratorV3>();
            Random = new Random();
            MinimumSize = 2;
            MaximumSize = 3;
        }

        /// <summary>
        /// Initializes a new <see cref="NameGeneratorV3">NameGenerator</see> 
        /// with specified symbol pools for the first and middle symbol positions of a syllable.
        /// Each character in the provided strings is considered a separate symbol.
        /// The generated syllables will be used for all positions of a name.
        /// </summary>
        public NameGeneratorV3(string firstSymbols, string middleSymbols) : this()
        {
            var syllableGenerator = new SyllableGeneratorV3()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Middle, middleSymbols);

            this.Set(SyllablePosition.Any, syllableGenerator);
        }

        /// <summary>
        /// Initializes a new <see cref="NameGeneratorV3">NameGenerator</see> 
        /// with specified symbol pools for the first, middle, and last symbol positions of a syllable.
        /// Each character in the provided strings is considered a separate symbol.
        /// The generated syllables will be used for all positions of a name.
        /// </summary>
        public NameGeneratorV3(string firstSymbols, string middleSymbols, string lastSymbols) : this()
        {
            var syllableGenerator = new SyllableGeneratorV3()
                .Add(SymbolPosition.First, firstSymbols)
                .Add(SymbolPosition.Middle, middleSymbols)
                .Add(SymbolPosition.Last, lastSymbols);

            this.Set(SyllablePosition.Any, syllableGenerator);
        }

        /// <summary>
        /// Sets the <see cref="SyllableGeneratorV3"/> for the specified position.
        /// </summary>
        /// <param name="position">The position for the syllable generator.</param>
        /// <param name="generator">The syllable generator to add.</param>
        /// <returns>The current instance of <see cref="NameGeneratorV3"/>.</returns>
        public NameGeneratorV3 Set(SyllablePosition position, SyllableGeneratorV3 generator)
        {
            if(position == SyllablePosition.Any)
            {
                Set(SyllablePosition.Leading, generator);
                Set(SyllablePosition.Inner, generator);
                Set(SyllablePosition.Trailing, generator);
            }
            else
            {
                SyllableGenerators[position] = generator;
            }

            return this;
        }

        /// <summary>
        /// Sets both the minimum and maximum number of syllables to use per name.
        /// </summary>
        public NameGeneratorV3 Size(int size)
        {
            MinimumSize = size;
            MaximumSize = size;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum number of syllables to use per name.
        /// </summary>
        public NameGeneratorV3 Size(int minSize, int maxSize)
        {
            MinimumSize = minSize;
            MaximumSize = maxSize;
            return this;
        }

        /// <summary>
        /// Generates a name by concatenating syllables from different positions based on the specified size.
        /// </summary>
        /// <returns>The generated name as a capitalized string.</returns>
        public string Next()
        {
            var result = GenerateName();
            return result;
        }

        private string GenerateName()
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
            var name = new StringBuilder();

            if (size == 1)
            {
                if (SyllableGenerators.ContainsKey(SyllablePosition.Leading))
                {
                    name.Append(SyllableGenerators[SyllablePosition.Leading].Next());
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
                    name.Append(SyllableGenerators[SyllablePosition.Leading].Next());
                    name.Append(SyllableGenerators[SyllablePosition.Trailing].Next());
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
                    name.Append(SyllableGenerators[SyllablePosition.Leading].Next());

                    for (int i = 1; i < size - 1; i++)
                    {
                        name.Append(SyllableGenerators[SyllablePosition.Inner].Next());
                    }

                    name.Append(SyllableGenerators[SyllablePosition.Trailing].Next());
                }
                else
                {
                    throw new InvalidOperationException("Not enough SyllableGenerators to create a name of the desired size.");
                }
            }

            // Capitalize the first character of the generated name
            return name[0].ToString().ToUpper() + name.ToString(1, name.Length - 1);
        }
    }
}
