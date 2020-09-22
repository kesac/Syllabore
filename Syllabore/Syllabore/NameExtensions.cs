using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    public static class NameExtensions
    {

        public static void ReplaceLeadingSyllable(this Name name, string syllable)
        {
            name.Syllables[0] = syllable;
        }

        public static void ReplaceTrailingSyllable(this Name name, string syllable)
        {
            name.Syllables[name.Syllables.Count - 1] = syllable;
        }

        public static string SyllableAt(this Name name, int index)
        {
            if(index < 0)
            {
                return name.Syllables[name.Syllables.Count + index];
            }
            else
            {
                return name.Syllables[index];
            }
        }

    }
}
