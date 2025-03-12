﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syllabore.Fluent;

namespace Syllabore.Example.Planets
{
    public class PlanetGeneratorV2_3 : NameGenerator
    {

        public PlanetGeneratorV2_3()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

            var f = new NameFilter();
            f.DoNotAllowEnding("f", "g", "j", "v");
            f.DoNotAllowRegex("([^aieou]{3})"); // Regex reads: non-vowels, three times in a row

            f.DoNotAllowRegex("(q[^u])"); // Q must always be followed by a u
            f.DoNotAllowRegex("([^tsao]w)"); // W must always be preceded with a t, s, a, or o

            // Some awkward looking combinations
            f.DoNotAllowSubstring("pn", "zz", "yy", "xx");
            f.DoNotAllowRegex("(y[^aeiou])"); // Avoids things like yt, yw, yz, etc.
            f.DoNotAllowRegex("(p[^aeioustrlh])"); // Avoids things like pb, pq, pz, etc.

            this.UsingFilter(f);
        }

    }
}
