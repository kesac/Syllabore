﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Validates names generated by a name generator.
    /// </summary>
    public interface INameFilter
    {
        /// <summary>
        /// Returns true if the specified name is a valid. Otherwise, it returns false.
        /// </summary>
        bool IsValid(Name name);
    }
}
