namespace Syllabore
{
    /// <summary>
    /// The type of condition that a 
    /// <see cref="FilterConstraint"/> uses.
    /// </summary>
    public enum FilterCondition
    {
        /// <summary>
        /// Condition is met if the name contains a specific substring.
        /// </summary>
        Contains,

        /// <summary>
        /// Condition is met if the name starts with a specific substring.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Condition is met if the name ends with a specific substring.
        /// </summary>
        EndsWith,

        /// <summary>
        /// Condition is met if the name matches a specific regular expression.
        /// </summary>
        MatchesPattern
    }
}
