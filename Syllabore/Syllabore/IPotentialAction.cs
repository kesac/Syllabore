using System;

namespace Syllabore
{
    /// <summary>
    /// An action that has a probability of occurring.
    /// </summary>
    public interface IPotentialAction
    {
        /// <summary>
        /// The probability that this action will occur.
        /// This value must be between 0 and 1 inclusive.
        /// A value of 0 means it will never occur. A value of 1 means it will always occur.
        /// Values between 0 and 1 represent a percentage chance of occurrence.
        /// </summary>
        double Chance { get; set; }
    }
}
