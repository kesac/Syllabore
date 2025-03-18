using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// A convenience wrapper for <see cref="SyllableGenerator"/> that
    /// is used by NameGenerator's fluent extension methods.
    /// </summary>
    public class SyllableGeneratorFluentWrapper
    {
        private NameGenerator _parent;
        private SyllablePosition _currentSyllablePosition;
        private SymbolPosition _lastSymbolPositionModified;

        /// <summary>
        /// The resulting <see cref="SyllableGenerator"/> after applying the fluent configuration.
        /// </summary>
        public SyllableGenerator Result { get; set; }

        /// <summary>
        /// Used by <see cref="NameGenerator"/> to instantiate a new <see cref="SyllableGeneratorFluentWrapper"/>.
        /// </summary>
        public SyllableGeneratorFluentWrapper(NameGenerator parent, SyllablePosition syllablePosition, SyllableGenerator syllables)
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
        public SyllableGeneratorFluentWrapper First(SymbolGenerator symbols) => Add(SymbolPosition.First, symbols);
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
        public SyllableGeneratorFluentWrapper Middle(SymbolGenerator symbols) => Add(SymbolPosition.Middle, symbols);
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
        public SyllableGeneratorFluentWrapper Last(SymbolGenerator symbols) => Add(SymbolPosition.Last, symbols);
        /// <summary>
        /// Adds symbols to the last position of the syllable.
        /// </summary>
        public SyllableGeneratorFluentWrapper Last(Func<SymbolGenerator, SymbolGenerator> configuration) => Add(SymbolPosition.Last, configuration(new SymbolGenerator()));

        /// <summary>
        /// Copies the SyllableGenerator from the leading position to the current syllable position.
        /// This method only works if the leading position is of type <see cref="SyllableGenerator"/>.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyLead() => CopyFrom(SyllablePosition.Leading);
        /// <summary>
        /// Copies the SyllableGenerator from the leading position to the current syllable position.
        /// This method only works if the leading position is of type <see cref="SyllableGenerator"/>.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyInner() => CopyFrom(SyllablePosition.Inner);
        /// <summary>
        /// Copies the SyllableGenerator from the leading position to the current syllable position.
        /// This method only works if the leading position is of type <see cref="SyllableGenerator"/>.
        /// </summary>
        public SyllableGeneratorFluentWrapper CopyTrail() => CopyFrom(SyllablePosition.Trailing);

        /// <summary>
        /// Sets the chance of generating a symbol for the last modified symbol position.
        /// </summary>
        public SyllableGeneratorFluentWrapper Chance(double chance)
        {
            Result.SetChance(_lastSymbolPositionModified, chance);
            return this;
        }

        private SyllableGeneratorFluentWrapper CopyFrom(SyllablePosition position)
        {
            if (_parent.SyllableGenerators.ContainsKey(position)
                && position != _currentSyllablePosition)
            {
                var syllableGenerator = _parent.SyllableGenerators[position] as SyllableGenerator;

                if (syllableGenerator != null)
                {
                    foreach (var pair in syllableGenerator.SymbolGenerators)
                    {
                        foreach (var symbols in pair.Value)
                        {
                            Result.Add(pair.Key, symbols.Copy());
                        }
                    }

                    foreach (var chance in syllableGenerator.PositionChance)
                    {
                        Result.SetChance(chance.Key, chance.Value);
                    }
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
