namespace Syllabore
{
    /// <summary>
    /// Represents the position of a symbol within a syllable.
    /// </summary>
    public enum SymbolPosition 
    {
        Unknown,

        /// <summary>
        /// The first symbol of a syllable.
        /// </summary>
        First,

        /// <summary>
        /// The position between the first and last symbols of a syllable.
        /// </summary>
        Middle,

        /// <summary>
        /// The last symbol of a syllable.
        /// </summary>
        Last,

        /// <summary>
        /// Represents any symbol position within a syllable or any syllable position within a name.
        /// </summary>
        Any
    }
}
