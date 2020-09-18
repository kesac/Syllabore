using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{

    /// <summary>
    /// Used by NameGenerator through ConfigurableNameMutator to capture
    /// mutations to produce variations on names. This class also has
    /// an optional condition that must be fulfilled for the mutation
    /// to occur.
    /// </summary>
    public class Mutation
    {
        public Action<Name> Mutate { get; set; }
        public Func<Name, bool> CanMutate { get; set; }
        public Mutation(Action<Name> mutate)
        {
            this.Mutate = mutate;
        }
    }
}
