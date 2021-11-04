using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates variations of a specified name.
    /// </summary>
    public interface INameTransformer
    {
        /// <summary>
        /// A number from 0 to 1 inclusive that represents the probablity
        /// that a NameGenerator mutator will apply a transform from this
        /// transformer during name generation (during a call to Next() or
        /// NextName()).
        /// A value of 0 means a mutation can never occur and a value of 1
        /// means a mutation will always occur.
        /// </summary>
        double? TransformChance { get; set; }

        Name Transform(Name sourceName);
    }
}
