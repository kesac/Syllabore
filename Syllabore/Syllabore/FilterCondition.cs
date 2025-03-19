namespace Syllabore
{
    /// <summary>
    /// The type of condition that a 
    /// <see cref="FilterConstraint"/> uses.
    /// </summary>
    public enum FilterCondition
    {
        Unknown = 0,

        /// <summary>
        /// Condition is met if the name contains a specific substring.
        /// </summary>
        Contains = 1,

        /// <summary>
        /// Condition is met if the name starts with a specific substring.
        /// </summary>
        StartsWith = 2,

        /// <summary>
        /// Condition is met if the name ends with a specific substring.
        /// </summary>
        EndsWith = 3,

        /// <summary>
        /// Condition is met if the name matches a specific regular expression.
        /// </summary>
        MatchesPattern = 4
    }
}
