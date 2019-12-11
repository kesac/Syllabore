using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public interface ISyllableModel
    {
        string NextVowel();
        string NextVowelCluster();
        string NextConsonant();
        string NextConsonantCluster();
        string NextCoda();
        string NextSyllable();
    }
}
