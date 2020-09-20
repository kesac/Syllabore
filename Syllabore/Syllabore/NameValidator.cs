using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// Validates names by checking them against a set of configurable constraints.
    /// </summary>
    public class NameValidator : IValidator
    {
        private List<Func<Name, bool>> Constraints { get; set; }

        public NameValidator()
        {
            this.Constraints = new List<Func<Name, bool>>();
        }
        public NameValidator Invalidate(Func<Name, bool> verify)
        {
            this.Constraints.Add(verify);
            return this;
        }

        /// <summary>
        /// Adds the specified constraint as a regular expression. Any name matching this contraint is considered invalid.
        /// </summary>
        public NameValidator InvalidateRegex(params string[] regex)
        {
            foreach (var r in regex) {
                this.Invalidate(x => Regex.IsMatch(x.ToString(), r));
            }
            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this validator's contraints, else returns false.
        /// </summary>
        public bool IsValidName(Name name)
        {

            bool isValid = true;

            foreach(var constraint in this.Constraints)
            {
                if(constraint(name))
                {
                    isValid = false;
                    break;
                }
            }

            return isValid;
        }
    }
}
