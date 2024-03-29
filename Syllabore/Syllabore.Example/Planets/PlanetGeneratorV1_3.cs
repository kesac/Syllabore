﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syllabore.Example.Planets
{

    public class PlanetGeneratorV1_3 : NameGenerator
    {
        public PlanetGeneratorV1_3()
        {
            this.UsingSyllableCount(2, 3);

            this.UsingSyllables(x => x
                .WithVowels("aieou")
                .WithLeadingConsonants("bcdfghklmnpqrstvxyz")
                .WithTrailingConsonants("cdfgklmnprstv")
                .WithProbability(x => x.OfTrailingConsonants(0.50)));

        }
    }
}
