using System;

namespace Syllabore
{
    /// <summary>
    /// Represents an entity that simulates
    /// randomness using system class <see cref="System.Random"/>.
    /// <para>
    /// The purpose of this interface is to make the entity's 
    /// instance of <see cref="System.Random"/>
    /// available for retrieval and modification. This is important
    /// for controlling seeds during testing.
    /// </para>
    /// </summary>
    public interface IRandomizable
    {
        /// <summary>
        /// The instance of <see cref="System.Random"/> used to 
        /// simulate randomness.
        /// </summary>
        Random Random { get; set; }
    }
}
