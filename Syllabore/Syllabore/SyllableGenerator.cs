using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables that can be sequenced into names.
    /// </summary>
    public class SyllableGenerator : ISyllableGenerator, IRandomizable
    {

        private SymbolGenerator _lastAddedGenerator;
        private SymbolPosition _lastPositionAdded;

        /// <summary>
        /// The instance of <see cref="System.Random"/> used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The symbol generators used to create new syllables.
        /// </summary>
        public Dictionary<SymbolPosition, List<SymbolGenerator>> SymbolGenerators { get; set; }

        /// <summary>
        /// The probability of generating a symbol for a given position.
        /// The default value is 1.0 (100%) for each position as long as there are symbols available.
        /// </summary>
        public Dictionary<SymbolPosition, double> PositionChance { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="SyllableGenerator"/> with no symbol generators.
        /// </summary>
        public SyllableGenerator()
        {
            Random = new Random();
            SymbolGenerators = new Dictionary<SymbolPosition, List<SymbolGenerator>>();
            PositionChance = new Dictionary<SymbolPosition, double>();

        }

        /// <summary>
        /// Adds symbols to the specified position. Each character in the string is considered a separate symbol.
        /// </summary>
        public SyllableGenerator Add(SymbolPosition position, string symbols) => Add(position, new SymbolGenerator().Add(symbols));

        /// <summary>
        /// Adds a <see cref="SymbolGenerator"/>. The generator's symbols will only
        /// be used for the specified position.
        /// </summary>
        public SyllableGenerator Add(SymbolPosition position, SymbolGenerator generator)
        {
            if (!SymbolGenerators.ContainsKey(position))
            {
                SymbolGenerators[position] = new List<SymbolGenerator>();

                if (!PositionChance.ContainsKey(position))
                {
                    PositionChance[position] = 1.0;
                }
            }

            SymbolGenerators[position].Add(generator);
            _lastAddedGenerator = generator;
            _lastPositionAdded = position;

            return this;
        }

        /// <summary>
        /// Sets the probability of generating a symbol for the specified position.
        /// The default value is 1.0 (100% probability) unless changed by calling this method.
        /// </summary>
        public SyllableGenerator SetChance(SymbolPosition position, double chance)
        {
            PositionChance[position] = chance;

            return this;
        }

        private string GenerateSymbol(SymbolPosition position)
        {
            if (SymbolGenerators.ContainsKey(position) && SymbolGenerators[position].Count > 0)
            {
                var generator = SymbolGenerators[position][Random.Next(SymbolGenerators[position].Count)];
                return generator.Next();
            }
            return string.Empty;
        }

        /// <summary>
        /// Generates a new syllable and returns it as a string.
        /// </summary>
        public string Next()
        {
            var syllable = new StringBuilder();

            // Generate symbols for each position based on their probabilities
            if (PositionChance.ContainsKey(SymbolPosition.First) && Random.NextDouble() < PositionChance[SymbolPosition.First])
            {
                syllable.Append(GenerateSymbol(SymbolPosition.First));
            }

            if (PositionChance.ContainsKey(SymbolPosition.Middle) && Random.NextDouble() < PositionChance[SymbolPosition.Middle])
            {
                syllable.Append(GenerateSymbol(SymbolPosition.Middle));
            }

            if (PositionChance.ContainsKey(SymbolPosition.Last) && Random.NextDouble() < PositionChance[SymbolPosition.Last])
            {
                syllable.Append(GenerateSymbol(SymbolPosition.Last));
            }

            if(syllable.Length == 0)
            {
                throw new InvalidOperationException("No symbols available to generate a syllable.");
            }

            return syllable.ToString();
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="SyllableGenerator"/>
        /// excluding internal instances of <see cref="System.Random"/>.
        /// </summary>
        public ISyllableGenerator Copy()
        {
            var newGenerator = new SyllableGenerator();
            
            foreach (var position in SymbolGenerators.Keys)
            {
                foreach (var symbols in SymbolGenerators[position])
                {
                    newGenerator.Add(position, symbols.Copy());
                }
            }

            foreach (var position in PositionChance.Keys)
            {
                newGenerator.SetChance(position, PositionChance[position]);
            }

            return newGenerator;
        }
    }
}
