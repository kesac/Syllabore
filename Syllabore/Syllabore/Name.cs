﻿using System.Collections.Generic;

namespace Syllabore
{
    /// <summary>
    /// Represents a name. Can be returned by a <see cref="INameGenerator"/> for callers
    /// to access individual syllables of a name.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// The ordered syllables that make up this name.
        /// </summary>
        public List<string> Syllables { get; set; }

        /// <summary>
        /// Instantiates an empty name.
        /// </summary>
        public Name()
        {
            this.Syllables = new List<string>();
        }

        /// <summary>
        /// Instantiates a new name with the desired starting syllables.
        /// </summary>
        /// <param name="syllable"></param>
        public Name(params string[] syllable)
        {
            this.Syllables = new List<string>(syllable);
        }

        /// <summary>
        /// Instantiates a new name that is a copy of the specified name. (This constructor
        /// is useful for a <see cref="INameTransformer"/>.)
        /// </summary>
        /// <param name="copy"></param>
        public Name(Name copy) {
            this.Syllables = new List<string>(copy.Syllables);
        }

        /// <summary>
        /// Sequences the syllables of this Name into a single
        /// string and then capitalizes it.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = string.Join(string.Empty, this.Syllables);

            if(result.Length > 0)
            {
                // Capitalize the first letter
                return result.Substring(0, 1).ToUpper() + result.Substring(1).ToLower();
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// A <see cref="Name"/> is equal to another
        /// <see cref="Name"/> if and only if their
        /// string representations are also equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Name name)
            {
                return (name.ToString() == this.ToString());
            }

            return false;
        }

        
        /// <summary>
        /// Returns a hash code for this <see cref="Name"/>.
        /// </summary>
        public override int GetHashCode() // Needs to exist for the Equals() override.
        {
            return base.GetHashCode();
        }
    }
}
