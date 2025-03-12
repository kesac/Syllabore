using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syllabore.Example.Spaceship;
using Syllabore.Example.Planets;
using Syllabore.Example.RandomString;
using System.Diagnostics;
using Syllabore.Fluent;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunFluentExample();
            PrintSeparator();
            RunConstructorExample();
            PrintSeparator();
            RunV2V3Comparison(300000);
        }

        private static void RunConstructorExample()
        {
            
            var n1 = new NameGeneratorV3("srnl", "ae");
                
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(n1.Next());
            }

            PrintSeparator();

            var n2 = new NameGeneratorV3("srnl", "ae", "lmt");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(n2.Next());
            }

        }

        private static void RunFluentExample()
        {
            var names = new NameGeneratorV3()
                .Lead(x => x
                    .First("s")
                    .Middle("a")
                    .Last(x => x
                        .Add("n").Weight(4)
                        .Add("m").Weight(1)))
                .Inner(x => x
                    .First("t")
                    .Middle("e")
                    .Last("l"))
                .Trail(x => x
                    .CopyLead()
                    .First("t").Chance(0.5))
                .Transform(x => x
                    .AppendSyllable("gard")
                    .InsertSyllable(0, "star"))
                .Size(3);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(names.Next());
            }

        }

        private static void RunV2V3Comparison(int attempts)
        {
            var stopwatch = Stopwatch.StartNew();
            var v2 = new NameGenerator("ae", "srnl");

            for (int i = 0; i < attempts; i++)
            {
                v2.Next();
            }

            stopwatch.Stop();
            var v2time = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            var v3 = new NameGeneratorV3("srnl", "ae");

            for (int i = 0; i < attempts; i++)
            {
                v3.Next();
            }

            stopwatch.Stop();
            var v3time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Comparison of {attempts} name generations:");
            Console.WriteLine($"V2: {v2time}ms");
            Console.WriteLine($"V3: {v3time}ms");

        }

        public static void PrintSeparator()
        {
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine();
        }

    }
}
