using System.Collections.Generic;

namespace Syllabore
{
    /// <summary>
    /// Represents a generated name. Can be returned by NameGenerator for callers
    /// to access the syllables of a name.
    /// </summary>
    public class Name
    {
        public List<string> Syllables { get; set; }

        /// <summary>
        /// An empty name.
        /// </summary>
        public Name()
        {
            this.Syllables = new List<string>();
        }

        /// <summary>
        /// A name with the desired starting syllables.
        /// </summary>
        /// <param name="syllable"></param>
        public Name(params string[] syllable)
        {
            this.Syllables = new List<string>(syllable);
        }

        /// <summary>
        /// A name that is a copy of the specified name. (This constructor
        /// is useful for mutators.)
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
            return result.Substring(0, 1).ToUpper() + result.Substring(1).ToLower();
        }

        public override bool Equals(object obj)
        {
            if(obj is Name){
                return (((Name)obj).ToString() == this.ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
