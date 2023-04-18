using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syllabore.Example.Spaceship;
using Syllabore.Example.Planets;
using Syllabore.Example.RandomString;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            {
                // You can use name generators without any customization.
                // A "default" name generator will use all vowels and
                // consonants of the English alphabet with no custom weighting
                // or filtering. Expect the output to be wild and exotic.

                var g = new NameGenerator();

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // To tailor name generation, specify
                // what vowels and consonants you want to use:
                var g = new NameGenerator("ae", "strml");

                // Or by explicitly declaring a SyllableGenerator first
                var s = new SyllableGenerator()
                            .WithVowels("ae")
                            .WithConsonants("strml");

                g = new NameGenerator(s);

                // Or in a more compact way...
                g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithConsonants("strmnl"));
            }
            {
                // Consonants can also be defined based
                // a desired positioning...
                var g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str")
                            .WithTrailingConsonants("mnl"));

                // ...In a syllable, leading consonants appear before
                // the vowel and trailing consonants appear after

            }
            {
                // You can also introduce vowel sequences or consonant
                // sequences that have a chance of being substituted in
                // for a normal vowel or consonant: 
                var g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithVowelSequences("ou", "ui")
                            .WithLeadingConsonants("str")
                            .WithLeadingConsonantSequences("wh", "fr")
                            .WithTrailingConsonants("mnl")
                            .WithTrailingConsonantSequences("ld","rd"));

                // Note that probabilities can be controlled. There is an
                // example of this further down.

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Separator();

                // This example is the same as the previous one except
                // it uses context-aware calls to Sequences() 
                g = new NameGenerator()
                    .UsingSyllables(x => x
                        .WithVowels("ae").Sequences("ou", "ui")
                        .WithLeadingConsonants("str").Sequences("wh", "fr")
                        .WithTrailingConsonants("mnl").Sequences("ld", "rd"));

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // The NameGenerator also allows you to define how often
                // sequence substitution occurs, how often leading or trailing consonants
                // occur, etc.
                var g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str")
                            .WithTrailingConsonants("mnl")
                            .WithProbability(x => x
                                .OfLeadingConsonants(1.0)
                                .OfTrailingConsonants(0.20)));

                // Or alternatively

                g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str")
                            .WithTrailingConsonants("mnl"))
                        .UsingProbability(x => x
                            .OfLeadingConsonants(1.0)
                            .OfTrailingConsonants(0.20));

                // In this example, the name generator will use syllables that always have
                // a leading consonant, but only have a trailing one 20% of the
                // time.

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {
                // You can also give custom weights to vowels
                // and consonants. A higher weight means a higher
                // frequency compared those with lower weights.
                var g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("a").Weight(4)
                            .WithVowels("e").Weight(1)
                            .WithLeadingConsonants("str"));

                // In this example, the vowel 'a' will appear 4 times
                // more likely than an 'e'

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

            }

            Separator();

            {

                // Filters can be used to improve output, by preventing
                // specific substrings or patterns from occuring:
                var f = new NameFilter()
                        .DoNotAllowStart("thr")               // Prevents "Thrond", but not "Athrun"
                        .DoNotAllowSubstring("quo", "tse")    // Prevents "Tsen", "Betsey", "Quon"
                        .DoNotAllowEnding("j", "p", "q", "w") // Prevents "Kaj", but not "Javal"
                        .DoNotAllow(@"(\w)\1\1");             // Prevents "Mareeen", but not "Mareen"
                
                var g = new NameGenerator()
                        .UsingFilter(f);

                // Or in a more compact way...
                g = new NameGenerator()
                        .DoNotAllow("j$", "p$", "q$", "w$")    // Invalidate these awkward endings
                        .DoNotAllow(@"(\w)\1\1")               // Invalidate any sequence of 3 or more identical letters
                        .DoNotAllow(@"([^aeiouAEIOU])\1\1\1"); // Invalidate any sequence of 4 or more 

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {

                // Transformers can be used to apply a transform
                // to a name during the generation process:
                var g = new NameGenerator()
                        .UsingTransform(new TransformSet()
                            .WithTransform(x => x.ReplaceSyllable(0, "te")).Weight(7)
                            .WithTransform(x => x.AppendSyllable("re")).Weight(3)
                            .RandomlySelect(1));


                g = new NameGenerator("aeo","str")
                    .UsingTransform(x => x
                        .When(0, "s").AppendSyllable("la"));

                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("! " + g.Next());
                }

                // Or simpler
                g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str"))
                        .UsingTransform(x => x
                            .AppendSyllable("gard"))
                        .UsingSyllableCount(3);


                // Or more complex
                g = new NameGenerator()
                        .UsingSyllables(x => x
                            .WithVowels("ae")
                            .WithLeadingConsonants("str"))
                        .UsingTransform(0.5, new TransformSet()
                            .WithTransform(x => x.AppendSyllable("gard")).Weight(2)
                            .WithTransform(x => x.AppendSyllable("dar"))
                            .RandomlySelect(1))
                        .UsingSyllableCount(3);

                // In this example, the name generation is creating
                // variations of the name *gard and *dar, with the former
                // being twice more likely to be generated.

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();

            }

            Separator();
            
            {
                var g = new NameGenerator();
                var m = new VowelShifter();

                for(int i = 0; i < 10; i++)
                {
                    var name = g.NextName();
                    var variation = m.Apply(name);

                    Console.WriteLine(name + "\t-> " + variation);
                }
                
            }

            Separator();
            

            {

                // This example shows a custom provider, transformer, and filter:
                Console.WriteLine();
                var g = new NameGenerator()
                        .UsingSyllables(p => p
                            .WithVowels("aeoy")
                            .WithLeadingConsonants("vstlr")
                            .WithTrailingConsonants("zrt")
                            .WithVowelSequences("ey", "ay", "oy"))
                        .UsingTransform(new TransformSet()
                            .RandomlySelect(1)
                            .WithTransform(x => x.ReplaceSyllable(0, "Gran"))
                            .WithTransform(x => x.ReplaceSyllable(0, "Bri"))
                            .WithTransform(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
                            .WithTransform(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
                            .WithTransform(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis")))
                        .UsingFilter(v => v
                            .DoNotAllow("yv", "yt", "zs")
                            .DoNotAllow(
                                @".{12,}",
                                @"(\w)\1\1",              // Prevents any letter from occuring three times in a row
                                @".*([y|Y]).*([y|Y]).*",  // Prevents double y
                                @".*([z|Z]).*([z|Z]).*")) // Prevents double z
                        .UsingSyllableCount(2, 4);

                var s = new NameGeneratorSerializer();

                s.Serialize(g, "city-name-generator.txt");
                var g2 = s.Deserialize("city-name-generator.txt");

                for (int i = 0; i < 10; i++)
                {
                    var name = g2.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }

            {

                // An example using >= v2.0.3 methods
                Console.WriteLine();
                var g = new NameGenerator()
                        .UsingSyllables(p => p // Overwrites previous call to .UsingCharacters()
                            .WithVowels("aeoy")
                            .WithLeadingConsonants("vstlr")
                            .WithTrailingConsonants("zrt")
                            .WithVowelSequences("ey", "ay", "oy"))
                        .UsingTransform(0.99, new TransformSet()
                            .RandomlySelect(1)
                            .WithTransform(x => x.ReplaceSyllable(0, "Gran"))
                            .WithTransform(x => x.ReplaceSyllable(0, "Bri"))
                            .WithTransform(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
                            .WithTransform(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
                            .WithTransform(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis")))
                        .DoNotAllow(
                            @"(\w)\1\1",             // Prevents any letter from occuring three times in a row
                            @".*([y|Y]).*([y|Y]).*", // Prevents double y
                            @".*([z|Z]).*([z|Z]).*") // Prevents double z
                        .UsingSyllableCount(2, 4);

                var s = new NameGeneratorSerializer();

                s.Serialize(g, "city-name-generator.txt");
                var g2 = s.Deserialize("city-name-generator.txt");

                for (int i = 0; i < 10; i++)
                {
                    var name = g2.NextName();
                    Console.WriteLine(name);
                }

                Console.WriteLine();

            }

            Separator();

            {
                // An example without using method chaining

                var syllables = new SyllableGenerator();
                syllables.WithVowels("a", "e", "o", "y");
                syllables.WithLeadingConsonants("v", "s", "t", "l", "r");
                syllables.WithTrailingConsonants("z", "r", "t");
                syllables.WithVowelSequences("ey", "ay", "oy");
                // provider.DisallowLeadingConsonantSequences(); // They will be off if nothing is defined
                // provider.DisallowTrailingConsonantSequences();  // They will be off if nothing is defined

                var shifter = new VowelShifter("a", "e", "o", "y");

                var filter = new NameFilter();
                filter.DoNotAllow(@"(\w)\1\1");
                filter.DoNotAllow(@"([^aeoyAEOY])\1");
                filter.DoNotAllow(@".*([y|Y]).*([y|Y]).*");
                filter.DoNotAllow(@".*([z|Z]).*([z|Z]).*");
                filter.DoNotAllow(@"(zs)");
                filter.DoNotAllow(@"(y[v|t])");

                var g = new NameGenerator(syllables, shifter, filter);
                g.UsingSyllableCount(2, 3);

            }

            Separator();

            {
                // Creating variations of a single name
                var name = new Name("syl", "la", "bore");
                var mutator = new TransformSet()
                                .Join(new TransformSet()
                                    .WithTransform(x => x.ReplaceSyllable(0, "test"))
                                .Join(new VowelShifter()));

                for(int i = 0; i < 5; i++)
                {
                    Console.WriteLine(mutator.Apply(name));
                }
            }

            Separator();

            {
                var g = new NameGenerator()
                        .UsingSyllables(new SyllableSet(2, 24, 2)
                            .WithStartingSyllable("ko", "ro")
                            .WithEndingSyllable("re", "ke")
                            .WithGenerator(x => x
                                .WithVowels("ae").Weight(2)
                                .WithVowels("iou")
                                .WithLeadingConsonants("str").Weight(2)
                                .WithLeadingConsonants("lmncvbyzwkd")))
                        .UsingSyllableCount(2,4);

                // In this example, the name generation is creating
                // variations of the name *gard and *dar, with the former
                // being twice more likely to be generated.

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }

                Console.WriteLine();
            }

            Separator();

            {
                var g = new JPGenerator();

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {
                // Fantasy-sounding names, maybe
                var g = new FantasyGenerator();

                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }
 
            Separator();

            {
                // Spaceship-sounding names, maybe
                var g = new SpaceshipGeneratorV5();

                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {
                // Spaceship-sounding names, maybe
                var g = new PlanetGeneratorV3();

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

            Separator();

            {
                // Random string generator
                var g = new RandomTextGenerator();

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next().ToUpper());
                }
            }

            Separator();

            {
                // eg. How to combine two NameGenerators
                var g = new NameFormatter("{firstname} {lastname}")
                        .UsingGenerator("firstname", new NameGenerator()
                            .UsingSyllableCount(2, 3)
                            .UsingSyllables(x => x
                                .WithConsonants("strlmn")
                                .WithVowels("aeo")))
                        .UsingGenerator("lastname", new NameGenerator()
                            .UsingSyllableCount(2, 4)
                            .UsingSyllables(x => x
                                .WithConsonants("bcdhky")
                                .WithVowels("oiyu")));

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine(g.Next());
                }
            }

        }

        public static void Separator()
        {
            Console.WriteLine("----------------------");
        }

    }
}
