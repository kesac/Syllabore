﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Validates names generated by an <see cref="INameGenerator"/>.
    /// </summary>
    public interface INameFilter
    {
        /// <summary>
        /// Returns true if the specified name is a valid. Otherwise, it returns false.
        /// </summary>
        bool IsValidName(Name name);
    }
}
