using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
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

        public NameMutator Join(NameMutator m)
        {
            this.Mutations.AddRange(m.Mutations);
            return this;
        }

        public NameMutator WithMutationCount(int limit)
        {
            this.MutationLimit = limit;
            return this;
        }


    }

}
