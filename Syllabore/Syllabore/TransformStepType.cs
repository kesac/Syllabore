namespace Syllabore
{
    /// <summary>
    /// The type of action that a <see cref="TransformStep"/> 
    /// will apply to a <see cref="Name"/>.
    /// </summary>
    public enum TransformStepType
    {
        /// <summary>
        /// Adds a syllable to a <see cref="Name"/>, displacing other
        /// syllables as needed.
        /// </summary>
        InsertSyllable,

        /// <summary>
        /// Adds a syllable to the end of a <see cref="Name"/>.
        /// </summary>
        AppendSyllable,

        /// <summary>
        /// Replaces a single syllable with a another syllable.
        /// </summary>
        ReplaceSyllable,

        /// <summary>
        /// Deletes a syllable from a <see cref="Name"/>, displacing
        /// other syllables as needed.
        /// </summary>
        RemoveSyllable,

        /// <summary>
        /// An action that is not serializable and expressed in a lambda.
        /// </summary>
        Lambda,

        /// <summary>
        /// Replaces all instances of a substring with another substring.
        /// </summary>
        ReplaceAllSubstring
    }
}
