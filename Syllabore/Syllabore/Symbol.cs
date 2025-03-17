using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Archigen;

namespace Syllabore
{
    /// <summary>
    /// An indivisible unit of a 
    /// writing system. In Syllabore, <see cref="Symbol">Symbols</see> 
    /// are used to represents vowels, consonants, sequences, or clusters.
    /// </summary>
    public class Symbol : IWeighted
    {
        /// <summary>
        /// A character or set of characters representing
        /// this <see cref="Symbol"/>.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A weight value that affects how frequently
        /// it should be selected compared to other
        /// weighted elements. The default weight
        /// of a <see cref="Symbol"/> is 1.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="Symbol"/> with the specified value.
        /// </summary>
        [JsonConstructor]
        public Symbol(string value) : this(value, 1) { }

        /// <summary>
        /// Instantiates a new <see cref="Symbol"/> with the specified value and weight.
        /// </summary>
        public Symbol(string value, int weight)
        {
            this.Value = value;
            this.Weight = weight;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="Symbol"/>.
        /// </summary>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="Symbol"/>.
        /// </summary>
        public Symbol Copy()
        {
            return new Symbol(this.Value) { Weight = this.Weight };
        }

    }
}
