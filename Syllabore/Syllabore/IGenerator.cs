﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// Generates names. Provides Name objects to allow inspection
    /// of syllables that were sequenced to make names or simple strings 
    /// if inspection is not required.
    /// </summary>
    interface IGenerator
    {
        string Next();
        string Next(int syllableLength);
        Name NextName();
        Name NextName(int syllableLength);
    }
}
