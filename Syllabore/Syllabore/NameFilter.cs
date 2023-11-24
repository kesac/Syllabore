using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{
    /// <summary>
    /// The type of condition that a 
    /// <see cref="FilterConstraint"/> uses.
    /// </summary>
    public enum FilterCondition
    {
        /// <summary>
        /// Condition is met if the name contains a specific substring.
        /// </summary>
        Contains,

        /// <summary>
        /// Condition is met if the name starts with a specific substring.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Condition is met if the name ends with a specific substring.
        /// </summary>
        EndsWith,

        /// <summary>
        /// Condition is met if the name matches a specific regular expression.
        /// </summary>
        MatchesPattern
    }

    /// <summary>
    /// A constraint used by a <see cref="NameFilter"/> when
    /// testing names for validity.
    /// </summary>
    public class FilterConstraint
    {
        /// <summary>
        /// The type of condition names will be tested against.
        /// (eg. Contains, StartsWith, EndsWith, MatchesPattern)
        /// </summary>
        public FilterCondition Type { get; set; }

        /// <summary>
        /// The value that names will be tested against (in conjunction
        /// with <see cref="Type"/>).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="FilterConstraint"/> with the specified condition and value.
        /// </summary>
        public FilterConstraint(FilterCondition type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

    }

    /// <summary>
    /// Validates names produced by a <see cref="INameGenerator"/> against a set of configurable constraints.
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
        /// Makes a name invalid if it contains any of the specified substrings.
        /// </summary>
        public NameFilter DoNotAllowSubstring(params string[] substring)
        {
            foreach(string s in substring)
            {
                this.Constraints.Add(new FilterConstraint(FilterCondition.Contains, s));
            }

            return this;
        }

        /// <summary>
        /// Makes a name invalid if it matches any of the specified regular expressions.
        /// </summary>
        public NameFilter DoNotAllow(params string[] regex)
        {
            foreach(string r in regex)
            {
                this.Constraints.Add(new FilterConstraint(FilterCondition.MatchesPattern, r));
            }

            return this;
        }

        /// <summary>
        /// Deprecated. Replaced with DoNotAllow().
        /// </summary>
        [Obsolete("Replaced with DoNotAllow()", false)]
        public NameFilter DoNotAllowPattern(params string[] regex)
        {
            return this.DoNotAllow(regex);
        }

        /// <summary>
        /// Makes a name invalid if it starts with any of the specified substrings.
        /// </summary>
        public NameFilter DoNotAllowStart(params string[] prefixes)
        {
            foreach (string s in prefixes)
            {
                this.Constraints.Add(new FilterConstraint(FilterCondition.StartsWith, s));
            }

            return this;
        }

        /// <summary>
        /// Makes a name invalid if it ends with any of the specified substrings.
        /// </summary>
        public NameFilter DoNotAllowEnding(params string[] suffixes)
        {
            foreach(string s in suffixes)
            {
                this.Constraints.Add(new FilterConstraint(FilterCondition.EndsWith, s));
                //this.Conditions.Add(s.Trim() + "$");
            }

            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this filter's contraints, else returns false.
        /// </summary>
        public bool IsValidName(Name name)
        {

            bool isValid = true;

            foreach (var c in this.Constraints)
            {
                var lowercaseName = name.ToString().ToLower();
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

                if(!isValid)
                {
                    break;
                }

            }
            return isValid;
        }

    }
}
