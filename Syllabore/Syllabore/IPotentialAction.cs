using System;

namespace Syllabore
{
    /// <summary>
    /// Represents an action that has a probability of occurring.
    /// </summary>
    public interface IPotentialAction
    {
        /// <summary>
        /// The probability that this action will occur.
        /// This value should be between 0 and 1 inclusive.
        /// </summary>
        double Chance { get; set; }
    }
}
