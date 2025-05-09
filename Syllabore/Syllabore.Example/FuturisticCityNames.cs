﻿using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    /// <summary>
    /// Names that sound like futuristic cities.
    /// </summary>
    public class FuturisticCityNames : Example
    {
        /// <summary>
        /// Generates names like: Zaut-31, Matica City, Go'kugo
        /// </summary>
        public IGenerator<string> GetGenerator()
        {
            var pool = new GeneratorPool<string>();

            pool.Add(GetDescriptiveCityGenerator());
            pool.Add(GetNumberedCityGenerator());
            pool.Add(GetAlienCityGenerator());

            return pool;
        }

        /// <summary>
        /// Generates names like: Zaut-31, Ort-77, Maut-623, Vyk-01
        /// </summary>
        public IGenerator<string> GetNumberedCityGenerator()
        {
            var digits = new SymbolGenerator("123456789");

            var names = new NameGenerator()
                .Start(x => x
                    .First("znmyvr")
                        .Chance(0.9)
                    .Middle(x => x
                        .Add("aieoy")
                        .Cluster("ae", "au", "ee", "ai"))
                    .Last(x => x
                        .Add("sktrx")
                        .Cluster("th", "rk", "st", "rt", "xt")))
                .Inner(x => x
                    .First("-"))
                .End(x => x
                    .First(digits).Chance(1.0)
                    .Middle(digits).Chance(0.66)
                    .Last(digits).Chance(0.33))
                .Transform(x => x
                    .When(-1, @"^.{1}$")
                    .Insert(-1, "0")) // Add a leading zero to single digit numbers
                .SetSize(3);

            return names;
        }

        /// <summary>
        /// Generates names like: Nora Moonbase, Matica City, Logio Colony
        /// </summary>
        public IGenerator<string> GetDescriptiveCityGenerator()
        {
            var names = new NameGenerator()
                .Start(x => x
                    .First(x => x
                        .Add("srlmn").Weight(5)
                        .Add("t").Weight(3)
                        .Add("bghkc").Weight(1))
                    .Middle(x => x
                        .Add("aeio").Weight(5)
                        .Cluster("io", "ou", "ie").Weight(3)
                        .Add("u").Weight(1)))
                .Inner(x => x.CopyStart())
                .End(x => x.CopyStart())
                .Transform(new TransformSet()
                    .Chance(0.2)
                    .RandomlySelect(1)
                    .Add(x => x.Replace(-1, "ron"))
                    .Add(x => x.Replace(-1, "byte"))
                    .Add(x => x.Replace(-1, "der")))
                .SetSize(2, 3);

            // 'RandomSelector' is from Archigen, a dependency of Syllabore
            var typeSelector = new RandomSelector<string>()
                .Add("city", "station", "prime")
                .Add("colony", "moonbase", "citadel");

            var formatter = new NameFormatter("{name}{type}")
                .Define("name", names)
                .Define("type", typeSelector, NameFormat.Capitalized, true);

            return formatter;
        }

        /// <summary>
        /// Generates names like: Radax, Tirasax, Darisaxis
        /// </summary>
        public IGenerator<string> GetAlienCityGenerator()
        {
            var names = new NameGenerator()
                .Start(x => x
                    .First("strd")
                    .Middle("ia"))
                .Inner(x => x
                    .CopyStart())
                .End(x => x
                    .CopyStart()
                    .Last("x"))
                .Transform(new TransformSet()
                    .Chance(0.2)
                    .RandomlySelect(1)
                    .Add(x => x.Append("is"))
                    .Add(x => x.Append("os")))
                .SetSize(2,3);

            return names;
        }


    }
}
