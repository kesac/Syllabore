using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates syllables that can be sequenced into names.
    /// </summary>
    public interface ISyllableGenerator : IGenerator<string>
    {
        /// <summary>
        /// Returns a copy of this <see cref="ISyllableGenerator"/>.
        /// </summary>
        ISyllableGenerator Copy();
    }
}
