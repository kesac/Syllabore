using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Represents a choice or entity that can
    /// be randomly selected from a list, and has 
    /// a weight value that affects how frequently it
    /// is selected compared to others.
    /// </summary>
    public interface IWeighted
    {
        int Weight { get; set; }
    }
}
