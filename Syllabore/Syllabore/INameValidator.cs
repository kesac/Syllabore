using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Used to validate randomly generated names.
    /// </summary>
    public interface INameValidator
    {
        /// <summary>
        /// Returns true if the specified name is a valid. Otherwise, it returns false.
        /// </summary>
        bool IsValidName(string name);
    }
}
