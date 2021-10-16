using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Syllabore
{

    public enum ConditionType
    {
        Contains,
        StartsWith,
        EndsWith,
        MatchesPattern
    }

    public class Condition
    {
        public ConditionType Type { get; set; }
        public string Value { get; set; }

        public Condition(ConditionType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

    }

    /// <summary>
    /// Validates names produced by a <see cref="NameGenerator"/> against a set of configurable constraints.
    /// </summary>
    [Serializable]
    public class NameValidator : IValidator
    {
        public List<Condition> Conditions { get; set; }

        public NameValidator()
        {
            this.Conditions = new List<Condition>();
        }

        public NameValidator DoNotAllow(params string[] substring)
        {
            foreach(string s in substring)
            {
                this.Conditions.Add(new Condition(ConditionType.Contains, s));
            }

            return this;
        }

        /// <summary>
        /// Adds the specified constraint as a regular expression. Any name matching this contraint is considered invalid.
        /// </summary>
        public NameValidator DoNotAllowPattern(params string[] regex)
        {
            foreach(string r in regex)
            {
                this.Conditions.Add(new Condition(ConditionType.MatchesPattern, r));
            }

            return this;
        }

        public NameValidator DoNotAllowStart(params string[] prefixes)
        {
            foreach (string s in prefixes)
            {
                this.Conditions.Add(new Condition(ConditionType.StartsWith, s));
                // this.Conditions.Add("^" + s.Trim());
            }

            return this;
        }

        public NameValidator DoNotAllowEnding(params string[] suffixes)
        {
            foreach(string s in suffixes)
            {
                this.Conditions.Add(new Condition(ConditionType.EndsWith, s));
                //this.Conditions.Add(s.Trim() + "$");
            }

            return this;
        }

        /// <summary>
        /// Returns true if the specified name does not match any of this validator's contraints, else returns false.
        /// </summary>
        public bool IsValidName(Name name)
        {

            bool isValid = true;

            foreach (var c in this.Conditions)
            {
                var lowercaseName = name.ToString().ToLower();
                var lowercaseValue = c.Value.ToLower();

                if (c.Type == ConditionType.Contains && lowercaseName.Contains(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == ConditionType.StartsWith && lowercaseName.StartsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == ConditionType.EndsWith && lowercaseName.EndsWith(lowercaseValue))
                {
                    isValid = false;
                }
                else if (c.Type == ConditionType.MatchesPattern && Regex.IsMatch(name.ToString(), c.Value, RegexOptions.IgnoreCase))
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
