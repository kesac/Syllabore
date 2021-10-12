using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// Validates names produced by a <see cref="NameGenerator"/> against a set of configurable constraints.
    /// </summary>
    [Serializable]
    public class NameValidator : IValidator
    {
        public List<string> InvalidPatterns { get; set; }

        public NameValidator()
        {
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

        public NameValidator DoNotAllowStart(params string[] regex)
        {
            foreach (string s in regex)
            {
                this.InvalidPatterns.Add("^" + s.Trim());
            }

            return this;
        }

        public NameValidator DoNotAllowEnding(params string[] regex)
        {
            foreach(string s in regex)
            {
                this.InvalidPatterns.Add(s.Trim() + "$");
            }

            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this validator's contraints, else returns false.
        /// </summary>
        public bool IsValidName(Name name)
        {

            bool isValid = true;

            foreach (var pattern in this.InvalidPatterns)
            {
                if (Regex.IsMatch(name.ToString(), pattern, RegexOptions.IgnoreCase))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }
    }
}
