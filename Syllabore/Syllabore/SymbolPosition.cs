namespace Syllabore
{
    /// <summary>
    /// The position of a symbol within a syllable.
    /// </summary>
    public enum SymbolPosition 
    {
        /// <summary>
        /// The symbol position is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The first symbol of a syllable.
        /// </summary>
        First = 1,

        /// <summary>
        /// The position between the first and last symbols of a syllable.
        /// </summary>
        Middle = 2,

        /// <summary>
        /// The last symbol of a syllable.
        /// </summary>
        Last = 3,

        /// <summary>
        /// Represents any symbol position within a syllable or any syllable position within a name.
        /// </summary>
        Any = 4
    }
}
