using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// Validates names by checking them against a set of configurable constraints.
    /// </summary>
    [Serializable]
    public class NameValidator : IValidator
    {
        // private List<Func<Name, bool>> Constraints { get; set; } // Flexible, but not serializable
        public List<string> InvalidPatterns { get; set; } // Only regex is serialized for now

        public NameValidator()
        {
            // this.Constraints = new List<Func<Name, bool>>();
            this.InvalidPatterns = new List<string>();
        }

        /// <summary>
        /// Adds the specified constraint as a regular expression. Any name matching this contraint is considered invalid.
        /// </summary>
        public NameValidator DoNotAllowPattern(params string[] regex)
        {
            this.InvalidPatterns.AddRange(regex);
            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this validator's contraints, else returns false.
        /// </summary>
        public bool IsValidName(Name name)
        {

            bool isValid = true;

            /*
            foreach(var constraint in this.Constraints)
            {
                if(constraint(name))
                {
                    isValid = false;
                    break;
                }
            }
            */

            foreach (var pattern in this.InvalidPatterns)
            {
                if (Regex.IsMatch(name.ToString(), pattern))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
    }
}
