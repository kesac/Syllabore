using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    /// <summary>
    /// Validates names produced by a name generator against a set of configurable constraints.
    /// </summary>
    [Serializable]
    public class NameFilter : INameFilter
    {
        /// <summary>
        /// The list of constraints that names must pass to be considered valid.
        /// </summary>
        public List<FilterConstraint> Constraints { get; set; }

        /// <summary>
        /// Instantiates a new <see cref="NameFilter"/> with no constraints.
        /// </summary>
        public NameFilter()
        {
            this.Constraints = new List<FilterConstraint>();
        }

        /// <summary>
        /// Adds new patterns to this filter.
        /// </summary>
        public NameFilter Add(params string[] patterns)
        {
            foreach (var value in patterns)
            {
                this.Add(new FilterConstraint(FilterCondition.MatchesPattern, value));

            }

            return this;
        }

        /// <summary>
        /// Adds a new constraint to this filter.
        /// </summary>
        public NameFilter Add(FilterConstraint constraint)
        {
            this.Constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this filter's contraints, else returns false.
        /// </summary>
        public bool IsValid(Name name)
        {
            return IsValid(name.ToString());
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this filter's contraints, else returns false.
        /// </summary>
        public bool IsValid(string name)
        {
            bool isValid = true;

            foreach (var c in this.Constraints)
            {
                var lowercaseName = name.ToLower();
                var lowercaseValue = c.Value.ToLower();

                if (c.Type == FilterCondition.Contains && lowercaseName.Contains(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == FilterCondition.StartsWith && lowercaseName.StartsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == FilterCondition.EndsWith && lowercaseName.EndsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == FilterCondition.MatchesPattern && Regex.IsMatch(name.ToString(), c.Value, RegexOptions.IgnoreCase))
                {
                    isValid = false;
                }

                if (!isValid)
                {
                    break;
                }

            }

            return isValid;
        }


    }
}
