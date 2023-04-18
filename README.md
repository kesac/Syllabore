[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

![](https://i.imgur.com/Y98oNli.png) 
#### What is this?
 * **Syllabore** is a procedural name generator and does not use pre-made lists of names
 * It can be embedded into a .NET program and used 100% offline

#### How are names generated?
 * **Syllabore** first constructs syllables out of characters (_graphemes_)
 * Then it sequences syllables into names

## Table of Contents
 1. [Quick Start](#quick-start)
 1. [Tailoring Characters](#tailoring-characters)
 1. [Filtering](#filtering-output)
 1. [Transformations](#transformations)
 1. [Installation](#installation)
 1. [Compatibility](#compatibility)
 1. [License](#license)

## Quick Start
Use the ```NameGenerator``` class to generate names. Call ``Next()`` to get a new name. By default, [all consonants and vowels in the English language](https://github.com/kesac/Syllabore/wiki/DefaultSyllableProvider) will be used in syllables. 

```csharp
var g = new NameGenerator();
Console.WriteLine(g.Next());
```
This will return names like:
```
Taigla
Zoren
Ocri
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
See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.1%EA%9E%89-Tailoring-Characters) for more examples on how to control things like vowel sequences, consonant positioning, and more!

## Filtering Output
Each ```NameGenerator``` can be configured to prevent specific substrings or patterns from showing up in names. Filtering is completely optional, but is useful in avoiding awkward sounding combinations of characters.

Here is a basic example of preventing substrings from appearing:
```csharp
var g = new NameGenerator()
        .DoNotAllow("ist") // Will prevent name like "Misty"
        .DoNotAllow("ck"); // Will prevent names like "Brock"
```

See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.2%EA%9E%89-Filtering-Output) for additional examples.

## Transformations
A ```Transform``` is a mechanism for changing a source name into a new, modified name. Call ```UsingTransform()``` on a ```NameGenerator``` to specify one or more transformations:
```csharp
var g = new NameGenerator()
        .UsingTransform(x => x
            .ReplaceSyllable(0, "zo") // Replace the first syllable
            .AppendSyllable("ri"));   // Adds a new syllable to end of name
```
Calling ```Next()``` produces names like:
```
Zocari
Zoshari
Zojiri
```
See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.3%EA%9E%89-Transformations) for additional examples.

## Installation
### .NET apps
Syllabore is available as a NuGet package. You can install it from your [NuGet package manager in Visual Studio](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

### Godot
Edit your ```.csproj``` file and add a ```PackageReference``` to Syllabore. Your file should look something like this:
```
<Project Sdk="Godot.NET.Sdk/4.0.1">
  ...
  <ItemGroup>
    <PackageReference Include="Syllabore" Version="2.0.2" />
  </ItemGroup>
  ...
</Project>
```

## Compatibility
Syllabore was created as a .NET Standard 2.0 class library. This means it will be immediately compatible with applications using:
 * .NET 5.0 and higher
 * .NET Core 2.0 and higher
 * .NET Framework 4.6.1 through 4.8.1
 * Mono 5.4, 6.4
 
Syllabore has been tested and known to work in the following game engines:
 * Godot 4 (Use the .NET version and reference Syllabore in your .csproj file)
 
Syllabore _should_ work in the following game engines, but I have not done adequate testing yet:
 * Unity

## License
```
MIT License

Copyright (c) 2019-2023 Kevin Sacro

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
