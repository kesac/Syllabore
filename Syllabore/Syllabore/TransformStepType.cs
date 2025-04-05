namespace Syllabore
{
    /// <summary>
    /// The type of action that a <see cref="TransformStep"/> 
    /// will apply to a <see cref="Name"/>.
    /// </summary>
    public enum TransformStepType
    {

        /// <summary>
        /// The step type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Adds a syllable to a <see cref="Name"/>, displacing other
        /// syllables as needed.
        /// </summary>
        InsertSyllable = 1,

        /// <summary>
        /// Adds a syllable to the end of a <see cref="Name"/>.
        /// </summary>
        AppendSyllable = 2,

        /// <summary>
        /// Replaces a single syllable with a another syllable.
        /// </summary>
        ReplaceSyllable = 3,

        /// <summary>
        /// Replaces all instances of a substring with another substring.
        /// </summary>
        ReplaceAllSubstring = 4,

        /// <summary>
        /// Deletes a syllable from a <see cref="Name"/>, displacing
        /// other syllables as needed.
        /// </summary>
        RemoveSyllable = 5,

        /// <summary>
        /// An action that is not serializable and expressed in a lambda.
        /// </summary>
        Lambda = 6,

    }
}
