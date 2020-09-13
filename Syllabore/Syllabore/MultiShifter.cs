using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Used to combine multiple name shifters into a single shifter.
    /// </summary>
    public class MultiShifter : IShifter
    {
        private Random Random;
        private List<IShifter> Shifters;

        public MultiShifter()
        {
            this.Random = new Random();
            this.Shifters = new List<IShifter>();
        }

        public MultiShifter Using(IShifter shifter)
        {
            this.Shifters.Add(shifter);
            return this;
        }

        public Name NextVariation(Name sourceName)
        {
            return this.Shifters[this.Random.Next(this.Shifters.Count)].NextVariation(sourceName);
        }
    }
}
