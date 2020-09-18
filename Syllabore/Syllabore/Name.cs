using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Represents a generated name. Can be returned by NameGenerator for callers
    /// to access the syllables of a name.
    /// </summary>
    public class Name
    {
        public List<string> Syllables { get; set; }

        public Name()
        {
            this.Syllables = new List<string>();
        }

        public Name(Name copy) {
            this.Syllables = new List<string>(copy.Syllables);
        }

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
