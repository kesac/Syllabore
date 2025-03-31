using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Transforms a <see cref="Name"/> into a new <see cref="Name"/>.
    /// </summary>
    public interface INameTransformer
    {
        /// <summary>
        /// Applies changes to the specified <see cref="Name"/>
        /// and returns a new <see cref="Name"/> as the result.
        /// </summary>
        Name Apply(Name sourceName);
    }
}
