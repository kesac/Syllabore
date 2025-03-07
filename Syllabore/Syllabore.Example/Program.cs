using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syllabore.Example.Spaceship;
using Syllabore.Example.Planets;
using Syllabore.Example.RandomString;
using System.Diagnostics;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
