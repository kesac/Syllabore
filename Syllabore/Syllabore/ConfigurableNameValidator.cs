using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// Validates names by checking them against a set of configurable constraints.
    /// </summary>
    public class ConfigurableNameValidator : INameValidator
    {
        private List<string> RegexConstraints { get; set; }

        public ConfigurableNameValidator()
        {
            this.RegexConstraints = new List<string>();
        }

        /// <summary>
        /// Adds the specified constraint as a regular expression. Any name matching this contraint is considered invalid.
        /// </summary>
        public ConfigurableNameValidator AddRegexConstraint(string regex)
        {
            this.RegexConstraints.Add(regex);
            return this;
        }


        /// <summary>
        /// Returns true if the specified name does not match any of this validator's contraints, else returns false.
        /// </summary>
        public bool IsValidName(string name)
        {

            bool isValid = true;

            foreach(var constraint in this.RegexConstraints)
            {
                if(Regex.IsMatch(name, constraint))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }
    }
}
