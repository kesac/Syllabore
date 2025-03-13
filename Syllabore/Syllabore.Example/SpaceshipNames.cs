using Archigen;
using System;
using System.Collections.Generic;
using System.Linq;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    /// <summary>
    /// Provides spaceship name generators
    /// </summary>
    public class SpaceshipNames
    {
        // Returns a NameGenerator that can generate names like:
        // VNH Mousiavium, NSH Ratirus, MNL Rousiades, etc.
        public IGenerator<string> GetGenerator()
        {
            var prefixes = new NameGenerator()
                .Any(x => x                    // For any syllable position...
                .First(x => x                  // For the first symbol of a syllable...
                    .Add("SHMLAMN").Weight(5)  // Choose these symbols most frequently
                    .Add("UVX").Weight(2)))    // Choose these symbols less frequently
                .Filter(@"(\w)\1\1")           // Prevent three consecutive identical characters
                .SetSize(3);

            var ships = new NameGenerator()
                .Any(x => x
                .First(x => x
                    .Add("rstlmn").Weight(4)
                    .Add("cdgp").Weight(2))
                .Middle(x => x
                    .Add("aoi")
                    .Cluster("ei", "ia", "ou", "eu")))
                .Transform(new TransformSet()
                    .RandomlySelect(1)
                    .Add(x => x.Replace(-1, "des"))
                    .Add(x => x.Replace(-1, "rus"))
                    .Add(x => x.Replace(-1, "vium")))
                .Filter(@"(\w)\1") 
                .SetSize(3);

            var formatter = new NameFormatter("{prefix} {name}")
                .Define("prefix", prefixes, NameFormat.UpperCase)
                .Define("name", ships);

            // You can call Next() on a NameFormatter to generate
            // random names like you would on a normal NameGenerator

            return formatter;
        }

        // Returns a name generator identical to GetGenerator(),
        // but without using Syllabore.Fluent extension methods
        public IGenerator<string> GetNonFluentGenerator()
        {
            // Part 1: Suffixes like "VNH", "NSH", "MNL"
            var prefixSymbols = new SymbolGenerator()
                .Add("SHMLAMN").Weight(5)
                .Add("UVX").Weight(2);

            var prefixSyllables = new SyllableGenerator()
                .Add(SymbolPosition.First, prefixSymbols);

            var shipPrefixes = new NameGenerator();
            shipPrefixes.SetSyllables(SyllablePosition.Any, prefixSyllables);
            shipPrefixes.Filter(@"(\w)\1\1");
            shipPrefixes.SetSize(3);

            // Part 2: Names like "Mousiavium", "Ratirus", "Rousiades"
            var shipVowels = new SymbolGenerator()
                .Add("aoi")
                .Cluster("ei", "ia", "ou", "eu");

            var shipConsonants = new SymbolGenerator()
                .Add("rstlmn").Weight(4)
                .Add("cdgp").Weight(2);

            var shipSyllables = new SyllableGenerator()
                .Add(SymbolPosition.First, shipConsonants)
                .Add(SymbolPosition.Middle, shipVowels);

            var shipNames = new NameGenerator();
            shipNames.SetSyllables(SyllablePosition.Any, shipSyllables);
            shipNames.Filter(@"(\w)\1"); // Remove two consecutive identical characters
            shipNames.SetSize(3);

            shipNames.SetTransform(new TransformSet()
                .RandomlySelect(1)
                .Add(new Transform().Replace(-1, "des"))
                .Add(new Transform().Replace(-1, "rus"))
                .Add(new Transform().Replace(-1, "vium")));

            // Part 3: Combine the two parts
            var formatter = new NameFormatter("{prefix} {name}");
            formatter.Define("prefix", shipPrefixes, NameFormat.UpperCase);
            formatter.Define("name", shipNames);

            return formatter;
        }

    }
}
