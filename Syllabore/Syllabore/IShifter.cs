using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public interface IShifter
    {
        Name NextVariation(Name sourceName);
    }
}
