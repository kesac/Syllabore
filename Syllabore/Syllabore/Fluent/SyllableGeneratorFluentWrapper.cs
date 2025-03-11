using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// A convenience wrapper for <see cref="SyllableGeneratorV3"/> that
    /// is used by NameGenerator's fluent extension methods.
    /// </summary>
    public class SyllableGeneratorFluentWrapper
    {
        private NameGeneratorV3 _parent;
        private SyllablePosition _currentSyllablePosition;
        private SymbolPosition _lastSymbolPositionModified;

        /// <summary>
        /// The resulting <see cref="SyllableGeneratorV3"/> after applying the fluent configuration.
        /// </summary>
        public SyllableGeneratorV3 Result { get; set; }

        /// <summary>
        /// Used by <see cref="NameGeneratorV3"/> to instantiate a new <see cref="SyllableGeneratorFluentWrapper"/>.
        /// </summary>
        public SyllableGeneratorFluentWrapper(NameGeneratorV3 parent, SyllablePosition syllablePosition, SyllableGeneratorV3 syllables)
        {
            _parent = parent;
            _currentSyllablePosition = syllablePosition;
            Result = syllables;
        }

        /// <summary>
        /// Adds symbols to the first position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper First(string symbols) => Add(SymbolPosition.First, new SymbolGenerator().Add(symbols));
        /// <summary>
        /// Adds symbols to the first position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper First(Func<SymbolGenerator, SymbolGenerator> configuration) => Add(SymbolPosition.First, configuration(new SymbolGenerator()));

        /// <summary>
        /// Adds symbols to the middle position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper Middle(string symbols) => Add(SymbolPosition.Middle, new SymbolGenerator().Add(symbols));
        /// <summary>
        /// Adds symbols to the middle position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper Middle(Func<SymbolGenerator, SymbolGenerator> configuration) => Add(SymbolPosition.Middle, configuration(new SymbolGenerator()));

        /// <summary>
        /// Adds symbols to the last position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper Last(string symbols) => Add(SymbolPosition.Last, new SymbolGenerator().Add(symbols));
        /// <summary>
        /// Adds symbols to the last position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper Last(Func<SymbolGenerator, SymbolGenerator> configuration) => Add(SymbolPosition.Last, configuration(new SymbolGenerator()));

        /// <summary>
        /// Copies the SyllableGenerator from the leading SyllableGenerator to the current syllable position.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyLead() => Use(SyllablePosition.Leading);
        /// <summary>
        /// Copies the SyllableGenerator from the leading SyllableGenerator to the current syllable position.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyInner() => Use(SyllablePosition.Inner);
        /// <summary>
        /// Copies the SyllableGenerator from the leading SyllableGenerator to the current syllable position.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyTrail() => Use(SyllablePosition.Trailing);

        /// <summary>
        /// Sets the chance of generating a symbol for the current syllable position.
        /// </summary>
        public SyllableGeneratorFluentWrapper Chance(double chance)
        {
            Result.Chance(_lastSymbolPositionModified, chance);
            return this;
        }

        private SyllableGeneratorFluentWrapper Use(SyllablePosition position)
        {
            if (_parent.SyllableGenerators.ContainsKey(position)
                && position != _currentSyllablePosition)
            {
                var syllableGenerator = _parent.SyllableGenerators[position];

                foreach (var pair in syllableGenerator.SymbolGenerators)
                {
                    foreach (var symbols in pair.Value)
                    {
                        Result.Add(pair.Key, symbols.Copy());
                    }
                }

                foreach (var chance in syllableGenerator.PositionChance)
                {
                    Result.Chance(chance.Key, chance.Value);
                }
            }

            return this;
        }

        private SyllableGeneratorFluentWrapper Add(SymbolPosition symbolPosition, SymbolGenerator generator)
        {
            _lastSymbolPositionModified = symbolPosition;
            Result.Add(symbolPosition, generator);
            return this;
        }

    }
}
