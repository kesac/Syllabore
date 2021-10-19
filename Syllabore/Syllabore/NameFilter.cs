using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    public enum Condition
    {
        Contains,
        StartsWith,
        EndsWith,
        MatchesPattern
    }

    public class Constraint
    {
        public Condition Type { get; set; }
        public string Value { get; set; }

        public Constraint(Condition type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

    }

    /// <summary>
    /// Validates names produced by a <see cref="NameGenerator"/> against a set of configurable constraints.
    /// </summary>
    [Serializable]
    public class NameFilter : IFilter
    {
        public List<Constraint> Constraints { get; set; }

        public NameFilter()
        {
            this.Constraints = new List<Constraint>();
        }

        /// <summary>
        /// Makes a name invalid if it contains any of the specified substrings.
        /// </summary>
        public NameFilter DoNotAllow(params string[] substring)
        {
            foreach(string s in substring)
            {
                this.Constraints.Add(new Constraint(Condition.Contains, s));
            }

            return this;
        }

        /// <summary>
        /// Makes a name invalid if it matches any of the specified regular expressions.
        /// </summary>
        public NameFilter DoNotAllowPattern(params string[] regex)
        {
            foreach(string r in regex)
            {
                this.Constraints.Add(new Constraint(Condition.MatchesPattern, r));
            }

            return this;
        }

        /// <summary>
        /// Makes a name invalid if it starts with any of the specified substrings.
        /// </summary>
        public NameFilter DoNotAllowStart(params string[] prefixes)
        {
            foreach (string s in prefixes)
            {
                this.Constraints.Add(new Constraint(Condition.StartsWith, s));
                // this.Conditions.Add("^" + s.Trim());
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
                this.Constraints.Add(new Constraint(Condition.EndsWith, s));
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

                if (c.Type == Condition.Contains && lowercaseName.Contains(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == Condition.StartsWith && lowercaseName.StartsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == Condition.EndsWith && lowercaseName.EndsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == Condition.MatchesPattern && Regex.IsMatch(name.ToString(), c.Value, RegexOptions.IgnoreCase))
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
