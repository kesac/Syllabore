using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            ExecutionTime(300000);
        }

        private static void RunConstructorExample()
        {
            
            var n1 = new NameGenerator("srnl", "ae");
                
            for(int i = 0; i < 10; i++)
            {
                Console.WriteLine(n1.Next());
            }

            PrintSeparator();

            var n2 = new NameGenerator("srnl", "ae", "lmt");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(n2.Next());
            }

        }

        private static void RunFluentExample()
        {
            var names = new NameGenerator()
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
                .Transform(new TransformSet()
                    .Add(x => x.AppendSyllable("gard"))
                    .Add(x => x.InsertSyllable(0, "star"))
                    .RandomlySelect(1))
                .Filter(x => x
                    .DoNotAllowStart("qi"))
                .Size(3);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(names.Next());
            }

        }

        private static void ExecutionTime(int attempts)
        {
            var stopwatch = Stopwatch.StartNew();
            var v2 = new NameGenerator("ae", "srnl");

            for (int i = 0; i < attempts; i++)
            {
                v2.Next();
            }

            stopwatch.Stop();
            var v2time = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Time for {attempts} name generations:");
            Console.WriteLine($"{v2time}ms");

        }

        public static void PrintSeparator()
        {
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine();
        }

    }
}
