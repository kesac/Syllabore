using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public interface IProvider
    {
        // Generate a random syllable suitable for starting name
        string NextStartingSyllable();

        // Generate a random syllable suitable for any part of a name
        string NextSyllable();

        // Generate a random syllable suitable for ending a name
        string NextEndingSyllable();

    }
}
