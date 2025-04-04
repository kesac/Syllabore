## Quick Start
Use the ```NameGenerator``` class to generate names. Call ``Next()`` to get a new name. By default, [a subset of consonants and vowels from the English language](https://github.com/kesac/Syllabore/wiki/DefaultSyllableGenerator) will be used. 

Here is an example .NET console application that generates a single name and prints it to the console:
```csharp
using Syllabore;

public class Program
{
    public static void Main(string[] args)
    {
        var g = new NameGenerator();
        Console.WriteLine(g.Next());
    }
}
```
This will return names like:
```
Pheras
Domar
Teso
```