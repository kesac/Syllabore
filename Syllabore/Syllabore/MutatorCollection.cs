using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Used to combine multiple name shifters into a single shifter.
    /// </summary>
    public class MutatorCollection : INameMutator
    {
        private Random Random;
        private List<INameMutator> Shifters;

        public MutatorCollection()
        {
            this.Random = new Random();
            this.Shifters = new List<INameMutator>();
        }

        public MutatorCollection Using(INameMutator shifter)
        {
            this.Shifters.Add(shifter);
            return this;
        }

        public Name Mutate(Name sourceName)
        {
            return this.Shifters[this.Random.Next(this.Shifters.Count)].Mutate(sourceName);
        }
    }
}
