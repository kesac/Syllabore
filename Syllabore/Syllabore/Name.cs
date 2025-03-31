using System.Collections.Generic;

namespace Syllabore
{
    /// <summary>
    /// Represents a sequence of syllables that make up a name.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// The ordered syllables that make up this name.
        /// </summary>
        public List<string> Syllables { get; set; }

        /// <summary>
        /// Creates an empty name with no syllables.
        /// </summary>
        public Name()
        {
            this.Syllables = new List<string>();
        }

        /// <summary>
        /// Creates a new name with the desired syllables.
        /// </summary>
        /// <param name="syllable"></param>
        public Name(params string[] syllable)
        {
            this.Syllables = new List<string>(syllable);
        }

        /// <summary>
        /// Instantiates a new name that is a copy of the specified <see cref="Name"/>. (This constructor
        /// is useful for a <see cref="INameTransformer"/>.)
        /// </summary>
        /// <param name="copy"></param>
        public Name(Name copy) {
            this.Syllables = new List<string>(copy.Syllables);
        }

        /// <summary>
        /// Adds a new syllable to this name. Returns this instance of <see cref="Name"/> for chaining.
        /// </summary>
        public Name Append(string syllable)
        {
            this.Syllables.Add(syllable);
            return this;
        }

        /// <summary>
        /// Sequences the syllables of this <see cref="Name"/> into a single
        /// string, capitalizes it, and returns it.
        /// </summary>
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
        /// Returns true if this <see cref="Name"/> is equal
        /// to the specified <see cref="Name"/>. 
        /// A <see cref="Name"/> is equal to another
        /// <see cref="Name"/> only if their
        /// string values are also equal.
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
