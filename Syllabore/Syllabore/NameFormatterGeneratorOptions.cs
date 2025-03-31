namespace Syllabore
{
    /// <summary>
    /// Stores options for controlling output of a formatted name.
    /// Used primarily by <see cref="NameFormatter"/>.
    /// </summary>
    public class NameFormatterGeneratorOptions
    {
        /// <summary>
        /// The desired format for names
        /// </summary>
        public NameFormat Format { get; set; }

        /// <summary>
        /// Whether the name should have a leading space at its beginning.
        /// </summary>
        public bool UseLeadingSpace { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="NameFormatterGeneratorOptions"/> with these default options: 
        /// capitalize names and don't use leading spaces.
        /// </summary>
        public NameFormatterGeneratorOptions()
        {
            this.Format = NameFormat.Capitalized;
            this.UseLeadingSpace = false;
        }

    }
}
