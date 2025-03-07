namespace Syllabore
{
    // TODO Distinguish between the position of a symbol in a syllable and a syllable's position within a nam
    /// <summary>
    /// Represents the position of a symbol within a syllable.
    /// </summary>
    public enum Position 
    {
        Unknown,

        /// <summary>
        /// <para>
        /// In a syllable, this is the first symbol usually occupied by a 
        /// leading consonant.
        /// </para>
        /// In a name, this is the first syllable of that name.
        /// </summary>
        First,

        /// <summary>
        /// <para>
        /// In a syllable, this is the position between the first and last symbols 
        /// normally used by a vowel.
        /// </para>
        /// In a name, this is the position between the first and last syllables.
        /// </summary>
        Middle,

        /// <summary>
        /// <para>
        /// In a syllable, this is the position between the first and last symbols 
        /// normally occupied by a trailing consonant.
        /// </para>
        /// In a name, this is the position between the first and last syllables.
        /// </summary>
        Last,

        /// <summary>
        /// Represents any symbol position within a syllable or any syllable position within a name.
        /// </summary>
        Any
    }
}
