namespace Syllabore
{
    /// <summary>
    /// A constraint used by a <see cref="NameFilter"/> when
    /// testing names for validity.
    /// </summary>
    public class FilterConstraint
    {
        /// <summary>
        /// The type of condition names will be tested against.
        /// (eg. Contains, StartsWith, EndsWith, MatchesPattern)
        /// </summary>
        public FilterCondition Type { get; set; }

        /// <summary>
        /// The value that names will be tested against (in conjunction
        /// with <see cref="Type"/>).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="FilterConstraint"/> with the specified condition and value.
        /// </summary>
        public FilterConstraint(FilterCondition type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

    }
}
