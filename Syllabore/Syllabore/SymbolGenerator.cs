using System;
using System.Collections.Generic;
using System.Linq;
using Archigen;

namespace Syllabore
{
    /// <summary>
    /// Generates random symbols from on a pool of weighted values.
    /// </summary>
    public class SymbolGenerator : IGenerator<string>, IRandomizable
    {
        private List<Symbol> _recentlyAddedSymbols;

        /// <summary>
        /// The instance of <see cref="System.Random"/> used to simulate randomness.
        /// </summary>
        public Random Random { get; set; }

        /// <summary>
        /// The possible <see cref="Symbol">Symbols</see> that can be generated
        /// by this generator.
        /// </summary>
        public List<Symbol> Symbols { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="SymbolGenerator"/>.
        /// </summary>
        public SymbolGenerator()
        {
            _recentlyAddedSymbols = new List<Symbol>();
            Symbols = new List<Symbol>();
            Random = new Random();
        }

        /// <summary>
        /// Adds the symbols in the provided string to this generator.
        /// Each character in the string is considered a separate symbol.
        /// </summary>
        public SymbolGenerator Add(string symbols)
        {
            var newSymbols = symbols.Atomize().Select(x => new Symbol(x)).ToList();

            Symbols.AddRange(newSymbols);
            _recentlyAddedSymbols.Clear();
            _recentlyAddedSymbols.AddRange(newSymbols);

            return this;
        }

        /// <summary>
        /// Adds provided symbol clusters (sequences) to this generator.
        /// Each argument regardless of its length is considered a single symbol.
        /// </summary>
        public SymbolGenerator Cluster(params string[] clusters)
        {
            var newSymbols = clusters.Select(x => new Symbol(x)).ToList();

            Symbols.AddRange(newSymbols);
            _recentlyAddedSymbols.Clear();
            _recentlyAddedSymbols.AddRange(newSymbols);

            return this;
        }

        /// <summary>
        /// Sets the weight of the most recently added symbols.
        /// This method should be called immediately after 
        /// <see cref="Add(string)"/> or <see cref="Cluster(string[])"/>.
        /// </summary>
        public SymbolGenerator Weight(int weight)
        {
            if(_recentlyAddedSymbols != null)
            {
                foreach(var symbol in _recentlyAddedSymbols)
                {
                    symbol.Weight = weight;
                }
            }

            return this;
        }

        /// <summary>
        /// Returns a random <see cref="Symbol"/> as a string. 
        /// <see cref="Symbol">Symbols</see> with higher weights are more likely
        /// to be selected.
        /// </summary>
        public string Next()
        {
            int totalWeight = Symbols.Sum(x => x.Weight);
            int randomSelection = Random.Next(totalWeight);

            int runningTotal = 0;

            for (int i = 0; i < Symbols.Count; i++)
            {
                runningTotal += Symbols[i].Weight;

                if (randomSelection < runningTotal)
                {
                    return Symbols[i].Value;
                }
            }

            throw new InvalidOperationException("A random symbol could not be selected because "
                + "there are none to choose from or there are elements with non-positive weights.");
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="SymbolGenerator"/>
        /// excluding the <see cref="SymbolGenerator.Random"/> property.
        /// </summary>
        public SymbolGenerator Copy()
        {
            var newGenerator = new SymbolGenerator();

            foreach (var symbol in Symbols)
            {
                newGenerator.Symbols.Add(symbol.Copy());
            }

            return newGenerator;
        }
    }
}
