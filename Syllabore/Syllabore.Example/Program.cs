using Syllabore.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System.Diagnostics;
using Syllabore.Fluent;
using Archigen;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunCustomGenerator(new HighFantasyNames().GetGenerator());
            PrintSeparator();

            RunCustomGenerator(new SpaceshipNames().GetGenerator());
            PrintSeparator();

            RunSimpleConstructorExample();
            PrintSeparator();

            PrintExecutionTime(300000);
        }

        private static void RunCustomGenerator(IGenerator<string> generator)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(generator.Next());
            }
        }

        private static void RunSimpleConstructorExample()
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

        private static void PrintExecutionTime(int attempts)
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
