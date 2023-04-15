using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    [Obsolete("Use ISyllableGenerator instead.", false)]
    public interface ISyllableProvider : ISyllableGenerator { }
}
