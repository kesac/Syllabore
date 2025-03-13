using Archigen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// A convenience class used for modeling names that have multiple parts
    /// and need multiple generators to create them.
    /// </summary>
    public class NameFormatter : IGenerator<string>
    {
        /// <summary>
        /// <para>
        /// The desired format for names. Surround substrings that need to be replaced with 
        /// a generated name with curly brackets.
        /// </para>
        /// <para>
        /// For example, the format <i>"John {middle-name} Smith"</i> tells a <see cref="NameFormatter"/> that the
        /// name between the first and last needs to be generated.
        /// </para>
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The <see cref="NameGenerator">NameGenerators</see>
        /// used by this <see cref="NameFormatter"/>.
        /// </summary>
        public Dictionary<string, NameGenerator> BoundNameGenerators { get; set; }

        /// <summary>
        /// Provides hints on whether a name should be upper case, lower case, capitalized, etc.
        /// </summary>
        public Dictionary<string, NameFormat> StringCaseTypes { get; set; }

        /// <summary>
        /// <para>
        /// Instantiates a new <see cref="NameFormatter"/> with the specified format. Substrings 
        /// that need to be replaced with a generated name should be surrounded with curly brackets.
        /// </para>
        /// <para>
        /// For example, the format <i>"John {middle-name} Smith"</i> tells a <see cref="NameFormatter"/> that the
        /// name between the first and last needs to be generated.
        /// </para>
        /// </summary>
        public NameFormatter(string format)
        {
            this.Format = format ?? throw new ArgumentNullException("format", "The desired format cannot be null");
            this.BoundNameGenerators = new Dictionary<string, NameGenerator>();
            this.StringCaseTypes = new Dictionary<string, NameFormat>();
        }

        /// <summary>
        /// Specifies a <see cref="NameGenerator"/> for the specified property.
        /// </summary>
        public NameFormatter Define(string property, NameGenerator generator, NameFormat stringCase = NameFormat.Capitalized)
        {
            this.BoundNameGenerators[property] = generator ?? throw new ArgumentNullException("generator", "The specified generator cannot be null");
            this.StringCaseTypes[property] = stringCase;
            return this;
        }

        /// <summary>
        /// Returns a new generated name based on the previously specified format.
        /// </summary>
        public string Next()
        {
            var result = new StringBuilder(this.Format);
            var properties = this.GetProperties(this.Format);

            foreach (var property in properties)
            {
                if (this.BoundNameGenerators.ContainsKey(property))
                {
                    var name = this.BoundNameGenerators[property].Next();

                    if (this.StringCaseTypes.ContainsKey(property))
                    {
                        if (this.StringCaseTypes[property] == NameFormat.UpperCase)
                        {
                            name = name.ToUpper();
                        }
                        else if (this.StringCaseTypes[property] == NameFormat.LowerCase)
                        {
                            name = name.ToLower();
                        }
                        else if (this.StringCaseTypes[property] == NameFormat.Capitalized)
                        {
                            // Name should already be capitalized by default
                        }
                    }

                    result.Replace("{" + property + "}", name);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Grabs substrings that were surrounded by curly brackets
        /// and returns them in an array.
        /// </summary>
        private string[] GetProperties(string format)
        {
            var matches = Regex.Matches(format, "\\{.+?\\}");
            var result = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                // Remove the leading and trailing curly bracket
                result[i] = matches[i].Value.Substring(1, matches[i].Length - 1 - 1);
            }

            return result;
        }

    }
}
