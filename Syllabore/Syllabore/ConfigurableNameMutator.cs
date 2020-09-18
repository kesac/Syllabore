using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{

    public class Mutation
    {
        public Action<Name> Mutate { get; set; }
        public Func<Name, bool> CanMutate { get; set; }

        public Mutation(Action<Name> mutate)
        {
            this.Mutate = mutate;
        }
    }

    public class ConfigurableNameMutator : INameMutator
    {
        private Random Random;
        private List<Mutation> Mutations;
        public int MutationLimit { get; private set; }

        public ConfigurableNameMutator()
        {
            this.Random = new Random();
            this.Mutations = new List<Mutation>();
            this.MutationLimit = 1;
        }

        public Name Mutate(Name sourceName)
        {
            var result = new Name(sourceName);
            for(int i = 0; i < this.MutationLimit; i++)
            {
                var mutation = this.Mutations[this.Random.Next(this.Mutations.Count)];

                if (mutation.CanMutate == null || mutation.CanMutate(result)) // Not all mutations have a condition
                {
                    mutation.Mutate(result);
                }
            }
            return result;
        }

        public ConfigurableNameMutator WithMutation(Action<Name> mutate)
        {
            this.Mutations.Add(new Mutation(mutate));
            return this;
        }

        public ConfigurableNameMutator When(Func<Name, bool> when)
        {
            this.Mutations[this.Mutations.Count - 1].CanMutate = when;
            return this;
        }

        public ConfigurableNameMutator WithMutationCount(int limit)
        {
            this.MutationLimit = limit;
            return this;
        }


    }

}
