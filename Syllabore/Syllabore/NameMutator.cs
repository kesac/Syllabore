using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    /// <summary>
    /// Takes source names and applies one or more mutations
    /// to create a name variations.
    /// </summary>
    [Serializable]
    public class NameMutator : IMutator
    {
        private Random Random;
        public List<Mutation> Mutations { get; set; }
        public int MutationLimit { get; set; }

        public NameMutator()
        {
            this.Random = new Random();
            this.Mutations = new List<Mutation>();
            this.MutationLimit = 1;
        }

        private Mutation GetWeightedSelection()
        {
            int totalWeight = this.Mutations.Sum(x => x.Weight);
            int selection = this.Random.Next(totalWeight);

            int runningTotal = 0;
            for (int j = 0; j < totalWeight; j++)
            {
                runningTotal += this.Mutations[j].Weight;
                if (selection < runningTotal)
                {
                    return this.Mutations[j];
                }
            }

            throw new InvalidOperationException("GetWeightedSelection() failed generated a selection that is in range of total weights");
        }

        public Name Mutate(Name sourceName)
        {
            var result = new Name(sourceName);

            for (int i = 0; this.Mutations.Count > 0 && i < this.MutationLimit; i++)
            {
                var mutation = this.GetWeightedSelection();
                var canApplyMutation = mutation.ConditionalRegex == null;

                if (mutation.ConditionalRegex != null)
                {
                    if(mutation.ConditionalIndex.HasValue)
                    {
                        int index = mutation.ConditionalIndex.Value;

                        if(index < 0) // reverse index provided, so translate into forward index (eg. -1 is the last syllable)
                        {
                            index = sourceName.Syllables.Count + index;
                        }

                        if(Regex.IsMatch(sourceName.Syllables[index], mutation.ConditionalRegex))
                        {
                            canApplyMutation = true;
                        }
                    }
                    else if (Regex.IsMatch(sourceName.ToString(), mutation.ConditionalRegex))
                    {
                        canApplyMutation = true;
                    }
                }

                if (canApplyMutation)
                {
                    mutation.Apply(result);
                }

            }
            
            return result;
        }

        public NameMutator WithMutation(Func<Mutation, Mutation> config)
        {
            this.Mutations.Add(config(new Mutation()));
            return this;
        }

        /// <summary>
        /// Applies a weight to the last added mutation that influences the probability of being used over others.
        /// Given two mutations X and Y with a weight of 3 and 1 respectively, mutation X will be applied 75% of the time.
        /// All mutations default to a weight of 1.
        /// </summary>
        public NameMutator Weight(int weight)
        {
            this.Mutations[this.Mutations.Count - 1].Weight = weight;
            return this;
        }

        public NameMutator Join(NameMutator m)
        {
            NameMutator newMutator = new NameMutator() { MutationLimit = this.MutationLimit };

            newMutator.Mutations.AddRange(this.Mutations);
            newMutator.Mutations.AddRange(m.Mutations);
            return newMutator;
        }

        public NameMutator WithMutationCount(int limit)
        {
            this.MutationLimit = limit;
            return this;
        }


    }

}
