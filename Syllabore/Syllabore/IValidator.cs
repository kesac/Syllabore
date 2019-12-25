using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public interface IValidator
    {
        bool IsValidName(string name);
    }
}
