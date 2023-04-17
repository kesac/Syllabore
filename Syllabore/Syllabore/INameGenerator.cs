using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates names as strings or <see cref="Name"/> objects.
    /// </summary>
    public interface INameGenerator : IGenerator<string>
    {
        /// <summary>
        /// Returns a new name of the specified syllable length.
        /// </summary>
        string Next(int syllableLength);

        /// <summary>
        /// Returns a new <see cref="Name"/>.
        /// </summary>
        Name NextName();

        /// <summary>
        /// Returns a new <see cref="Name"/> of the specified syllable length.
        /// </summary>
        Name NextName(int syllableLength);
    }
}
