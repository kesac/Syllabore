using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates variations of a specified name.
    /// </summary>
    public interface IShifter
    {
        Name NextVariation(Name sourceName);
    }
}
