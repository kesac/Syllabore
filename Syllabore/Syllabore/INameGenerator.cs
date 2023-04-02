using Archigen;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates names as strings or <see cref="Name"/> objects.
    /// </summary>
    public interface INameGenerator : IGenerator<string>
    {
        // string Next();
        string Next(int syllableLength);
        Name NextName();
        Name NextName(int syllableLength);
    }
}
