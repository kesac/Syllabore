[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

![](https://i.imgur.com/Y98oNli.png) 
### What is this?
Syllabore is a name generator that does not use pre-defined lists of names.

You will find Syllabore useful if:
- You do not want to randomly select names from lists
- You want a name generator that can be used 100% offline
- You want a name generator that can be embedded into a .NET app or game

## Quick Start

Use the `NameGenerator` class to generate names. Every call to `Next()` returns a new name. 

The following example creates a single name and prints it to the console:
```csharp
var names = new NameGenerator("str", "aeo");
Console.WriteLine(names.Next());
```
Each call to `names.Next()` will return names like:
```
Sasara
Rosa
Tetoro
```
Check out [the documentation](https://sacro.gitbook.io/syllabore) for more details.

## Positioning
Names are made up of syllables. Syllables are made up of symbols. 

Tell the `NameGenerator` what symbols to use for each *symbol position* and *syllable position*.
```csharp
var names = new NameGenerator()
    .Start(x => x     // The starting syllable of a name
        .First("st")  // Leading consonants
        .Middle("ae") // Vowels
        .Last("rl"))  // Trailing consonants
    .Inner(x => x     // The "body" of a name
        .First("mnr")
        .Middle("ioa"))
    .End(x => x       // The ending syllable of a name
        .CopyInner()) // Use the same symbols as inner syllables
    .SetSize(3);      // Makes names 3 syllables long
```
Calls to `names.Next()` will generate names like
```
Termino
Sarnina
Telnari
```
Check out [the documentation](https://sacro.gitbook.io/syllabore) for more details.

## Transforms
Add determinism to names by using a `transform`.

This example forces every name to end with the suffix `-nia`:

```csharp
var names = new NameGenerator()
    .Any(x => x                       // For all syllable positions...
        .First("lmnstr")              // Use these consonants
        .Middle("aeiou"))             // And these vowels
    .Transform(x => x.Append("nia"))  // Then add this suffix to the final name
    .SetSize(2);
```
Calls to `names.Next()` will produce names like:
```
Sarunia
Timania
Lisonia
```
Check out [the documentation](https://sacro.gitbook.io/syllabore) for more details.

## Filtering Output
Prevent certain symbol combinations from appearing in names by using a `filter`.

The following generator uses the symbols `m` `u`, but uses a filter to prevent `m` from appearing at the beginning of a name and prevents `u` from ending it:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("strlmn")
        .Middle("aeiou"))
    .Filter("^M|u$");
```
This generator produces names like:
```
Temaro
Rima
Narumi
```

Check out [the documentation](https://sacro.gitbook.io/syllabore) for more details.

## Installation
Check out [the installation documentation](https://sacro.gitbook.io/syllabore#how-do-i-add-syllabore-to-my-project) for full details. The gist is:
- In Visual Code, use the `NuGet: Add NuGet Package` command in Microsoft's C# Dev Kit extension to find `Syllabore`
- In Visual Studio, use the NuGet package manager to find `Syllabore` or run the package manager command `Install-Package Syllabore`
- In the .NET CLI, run the command `dotnet add package Syllabore`

## Compatibility
Syllabore is a [.NET Standard](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0) 2.0 class library and is compatible with applications using:

* .NET or .NET Core 2.0 to 8.0 inclusive
* .NET Framework 4.6.1 to 4.8.1 inclusive
* [Mono](https://www.mono-project.com/) 5.4 and 6.4
* [Godot 4](https://godotengine.org/download/windows/) (Using the .NET edition of the engine)
* [Unity Engine](https://unity.com/products/unity-engine)
* [MonoGame](https://www.monogame.net/)

## License
```
MIT License

Copyright (c) 2019-2025 Kevin Sacro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
