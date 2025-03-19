using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Specifies the format of a name.
    /// </summary>
    public enum NameFormat
    {
        Unknown = 0,

        /// <summary>
        /// Specifies a name in all lowercase characters.
        /// </summary>
        LowerCase = 1,

        /// <summary>
        /// Specifies a name in all uppercase characters.
        /// </summary>
        UpperCase = 2,

        /// <summary>
        /// Specifies a name with the first character capitalized
        /// and all other characters in lowercase.
        /// </summary>
        Capitalized = 4
    }
}
