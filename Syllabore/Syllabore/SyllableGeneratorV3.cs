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
    public class SyllableGeneratorV3 : IGenerator<string>, IRandomizable
    {

        private SymbolGenerator _lastAddedGenerator;
        private Position _lastPositionAdded;

        /// <summary>
        /// The instance of <see cref="System.Random"/> used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The symbol generators used to create new syllables.
        /// </summary>
        public Dictionary<Position, List<SymbolGenerator>> SymbolGenerators { get; set; }

        /// <summary>
        /// The probability of generating a symbol for a given position.
        /// The default value is 1.0 (100%) for each position as long as there are symbols available.
        /// </summary>
        public Dictionary<Position, double> PositionChance { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="SyllableGeneratorV3"/> with no symbol generators.
        /// </summary>
        public SyllableGeneratorV3()
        {
            Random = new Random();
            SymbolGenerators = new Dictionary<Position, List<SymbolGenerator>>();
            PositionChance = new Dictionary<Position, double>();

        }

        /// <summary>
        /// Adds symbols to the specified position. Each character in the string is considered a separate symbol.
        /// </summary>
        public SyllableGeneratorV3 Add(Position position, string symbols) => Add(position, new SymbolGenerator().Add(symbols));

        /// <summary>
        /// Adds a <see cref="SymbolGenerator"/>. The generator's symbols will only
        /// be used for the specified position.
        /// </summary>
        public SyllableGeneratorV3 Add(Position position, SymbolGenerator generator)
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
        public SyllableGeneratorV3 Chance(Position position, double chance)
        {
            PositionChance[position] = chance;

            return this;
        }

        private string GenerateSymbol(Position position)
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
            if (PositionChance.ContainsKey(Position.First) && Random.NextDouble() < PositionChance[Position.First])
            {
                syllable.Append(GenerateSymbol(Position.First));
            }

            if (PositionChance.ContainsKey(Position.Middle) && Random.NextDouble() < PositionChance[Position.Middle])
            {
                syllable.Append(GenerateSymbol(Position.Middle));
            }

            if (PositionChance.ContainsKey(Position.Last) && Random.NextDouble() < PositionChance[Position.Last])
            {
                syllable.Append(GenerateSymbol(Position.Last));
            }

            if(syllable.Length == 0)
            {
                throw new InvalidOperationException("No symbols available to generate a syllable.");
            }

            return syllable.ToString();
        }
    }
}
