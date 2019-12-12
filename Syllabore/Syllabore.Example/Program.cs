using System;

namespace Syllabore.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var provider = new BasicSyllableProvider();
            var validator = new BasicNameValidator();

            // Syllabore names = new Syllabore(model);
            Syllabore names = new Syllabore(provider, validator);

            for (int i = 0; i < 100; i++)
            {
                System.Console.WriteLine(names.Next());
            }
        }
    }
}
