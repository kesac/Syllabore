using System;
using System.Collections.Generic;
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

        public Name Mutate(Name sourceName)
        {
            var result = new Name(sourceName);

            for (int i = 0; this.Mutations.Count > 0 && i < this.MutationLimit; i++)
            {
                // Currently chooses a random mutation
                var mutation = this.Mutations[this.Random.Next(this.Mutations.Count)];
                var canApplyMutation = false;

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
                else
                {
                    canApplyMutation = true;
                }

                if (canApplyMutation)
                {
                    mutation.Apply(result);
                }

                /**
                if (mutation.CanMutate == null || mutation.CanMutate(result)) // Not all mutations have a condition
                {
                    mutation.Apply(result);
                }
                **/
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

        /*
        public NameMutator When(Func<Name, bool> when)
        {
            this.Mutations[this.Mutations.Count - 1].CanMutate = when;
            return this;
        }
        /**/

        public NameMutator WithMutationCount(int limit)
        {
            this.MutationLimit = limit;
            return this;
        }


    }

}
