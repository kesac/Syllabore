[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

![](https://i.imgur.com/Y98oNli.png) 
#### What is this?
 * **Syllabore** is a procedural name generator that does not use a pre-made lists of names
 * It can be embedded into a .NET program and used 100% offline

#### How are names generated?
 * **Syllabore** first constructs syllables out of characters (_graphemes_)
 * Then it sequences syllables into names

## Table of Contents
 1. [Quick Start](#quick-start)
 1. [Tailoring Characters](#tailoring-characters)
 1. [Transformations](#transformations)
 1. [Filtering](#filtering-output)
 1. [Installation](#installation)
 1. [Compatibility](#compatibility)
 1. [License](#license)

## Quick Start
Use the ```NameGenerator``` class to generate names. Call ``Next()`` to get a new name. By default, [a subset of consonants and vowels from the English language](https://github.com/kesac/Syllabore/wiki/What-is-the-DefaultSyllableGenerator) will be used. 

```csharp
var g = new NameGenerator();
Console.WriteLine(g.Next());
```
This will return names like:
```
Pheras
Domar
Teso
```

## Tailoring Characters
For simple generators, you can supply the vowels and consonants to use through the constructor:
```csharp
var g = new NameGenerator("ae", "srnl");   
```
Names from this generator will only ever have the characters `a` and `e` for vowels, and the characters `s` `r` `n` and `l` for consonants. Calls to ```Next()``` will produce names like:
```
Lena
Salna
Rasse
```
See the [wiki](https://github.com/kesac/Syllabore/wiki) for more examples on how to control things like vowel sequences, consonant positioning, and more!

## Transformations
A ```Transform``` is a mechanism for changing a source name into a new, modified name. Call ```UsingTransform()``` on a ```NameGenerator``` to specify one or more transformations:
```csharp
var g = new NameGenerator()
        .UsingTransform(x => x
            .ReplaceSyllable(0, "zo") // Replace the first syllable with string "zo"
            .AppendSyllable("ri"));   // Adds a new syllable to end of name
```
Calling ```Next()``` produces names like:
```
Zocari
Zoshari
Zojiri
```
See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.3%EA%9E%89-Transformations) for additional examples.

## Filtering Output
Each ```NameGenerator``` can be configured to prevent specific substrings or patterns from showing up in names. Filtering is completely optional, but is useful in avoiding awkward sounding combinations of characters.

Here is a basic example of preventing substrings from appearing:
```csharp
var g = new NameGenerator()
        .DoNotAllow("ist") // Will prevent names like "Misty"
        .DoNotAllow("ck"); // Will prevent names like "Brock"
```

See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.2%EA%9E%89-Filtering-Output) for additional examples.


## Installation
### [.NET apps](https://learn.microsoft.com/en-us/dotnet/core/introduction) 
Syllabore is available as a [NuGet](https://learn.microsoft.com/en-us/nuget/what-is-nuget) package. You can install it from your [NuGet package manager in Visual Studio](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

### [Godot Engine](https://godotengine.org/)
There are a couple ways to do this in [Godot](https://godotengine.org/):
- Open your Godot project in Visual Studio and add the Syllabore NuGet package through the [package manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio)
- Or open a command line, `cd` into your Godot project directory, and use the following command:
```
dotnet add package Syllabore
```

## Compatibility
By design, Syllabore is a [.NET Standard](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0) 2.0 class library. This means it will be compatible with applications using:
 * .NET or .NET Core 2.0, 2.1, 2.2, 3.0, 3.1, 5.0, 6.0, 7.0, 8.0
 * .NET Framework 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8, 4.8.1
 * [Mono](https://www.mono-project.com/) 5.4, 6.4
 
Syllabore has been tested and known to work in the following game engines:
 * [Godot 4](https://godotengine.org/download/windows/) (Using the .NET edition of the engine)
 
Syllabore should also work in the following game engines, but I have not done adequate testing yet:
 * [Godot 3](https://godotengine.org/download/3.x/windows/)
 * [Unity Engine](https://unity.com/products/unity-engine)
 * [MonoGame](https://www.monogame.net/)

## License
```
MIT License

Copyright (c) 2019-2024 Kevin Sacro

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
