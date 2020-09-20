﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            { 
                // Quickest way to use Syllabore's name generator
                // without specifying any configuration. This instance
                // will default to using StandaloneSyllableProvider for
                // name generator and will not use any NameValidator to
                // improve output.
                var g = new NameGenerator();

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }
            {

                // Normally the constructor takes a SyllableProvider
                // and NameValidator. There are "Standalone" classes
                // available for quick and dirty use. It is recommended
                // you create your own by using ISyllableProvider/INameValidator
                // or inheriting from ConfigurableSyllableProvider/ConfigurableNameValidator.

                var provider = new DefaultSyllableProvider();
                var validator = new ConfigurableNameValidator()
                        .InvalidateRegex(@"[j|p|q|w]$")             // Invalidate these awkward endings
                        .InvalidateRegex(@"(\w)\1\1")               // Invalidate any sequence of 3 or more identical letters
                        .InvalidateRegex(@"([^aeiouAEIOU])\1\1\1"); // Invalidate any sequence of 4 or more consonants
                
                var g = new NameGenerator(provider, validator);

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            {
                // Configuration of syllable providers and name validators
                // can be captured in a dedicated XML file then loaded
                // through the XmlFileLoader.

                var file = new XmlFileLoader("data/basic.xml").Load();
                var g = file.GetNameGenerator("SoftNameGenerator").LimitSyllableCount(2, 4);

                Console.WriteLine();
                for (int i = 0; i < 10; i++)
                {
                    System.Console.WriteLine(g.Next());
                }
                Console.WriteLine();
            }
            {

                // If you don't like XML, you can choose to
                // build name generators programmatically.
                var g = new NameGenerator()
                    .UsingProvider(x => x
                        .WithLeadingConsonants("str")
                        .WithVowels("ae"))
                    .LimitSyllableCount(3);

                for(int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();

            }

            {
                // Creating variations of a single name
                var g = new NameGenerator()
                    .UsingMutator(new MutatorCollection()
                        .Add(new DefaultNameMutator())
                        .Add(new VowelMutator()));

                for(int i = 0; i < 3; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);

                    for (int j = 0; j < 4; j++)
                    {
                        var variation = g.NextVariation(name);
                        Console.WriteLine(variation);

                    }
                }
            }


            {
                Console.WriteLine();
                var g = new NameGenerator()
                    .UsingProvider(p => p
                        .WithVowels("aeoy")
                        .WithLeadingConsonants("vstlr")
                        .WithTrailingConsonants("zrt")
                        .WithVowelSequences("ey", "ay", "oy"))
                    .UsingMutator(m => m
                        .WithMutation(x => { x.Syllables[0] = "Gran"; })
                        .WithMutation(x => x.Syllables.Add("opolis")).When(x => x.EndsWithConsonant())
                        .WithMutation(x => x.Syllables.Add("polis")).When(x => x.EndsWithVowel())
                        .WithMutationCount(1))
                    .UsingValidator(v => v
                        .Invalidate(x => x.ToString().Length > 12)
                        .InvalidateRegex(
                            @"(\w)\1\1",             // no letters three times in a row
                            @".*([y|Y]).*([y|Y]).*", // two y's in same name
                            @".*([z|Z]).*([z|Z]).*", // two z's in same name
                            @"(zs)",                 // this just looks weird
                            @"(y[v|t])"))            // this also looks weird 
                    .LimitMutationChance(0.99)
                    .LimitSyllableCount(2, 3);
                

                for (int i = 0; i < 50; i++)
                {
                    var name = g.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }
            {
                var provider = new ConfigurableSyllableProvider();
                provider.WithVowels("a", "e", "o", "y");
                provider.WithLeadingConsonants("v", "s", "t", "l", "r");
                provider.WithTrailingConsonants("z", "r", "t");
                provider.WithVowelSequences("ey", "ay", "oy");
                provider.DisallowLeadingConsonantSequences();
                provider.DisallowTrailingConsonantSequences();

                var shifter = new VowelMutator("a", "e", "o", "y");

                var validator = new ConfigurableNameValidator();
                validator.InvalidateRegex(@"(\w)\1\1");
                validator.InvalidateRegex(@"([^aeoyAEOY])\1");
                validator.InvalidateRegex(@".*([y|Y]).*([y|Y]).*");
                validator.InvalidateRegex(@".*([z|Z]).*([z|Z]).*");
                validator.InvalidateRegex(@"(zs)");
                validator.InvalidateRegex(@"(y[v|t])");

                var g = new NameGenerator(provider, shifter, validator);
                g.LimitSyllableCount(2, 3);

            }

        }
    }
}
