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
 1. [Transformations](#transformations)
 1. [Filtering](#filtering-output)
 1. [Putting It All Together](#putting-it-all-together)
 1. [Serialization](#serialization)
 1. [Installation](#installation)
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
Each ```NameGenerator``` uses a ```SyllableProvider``` internally. You can specify the characters (_graphemes_) to use in name generation by supplying your own ```SyllableProvider```:
```csharp
var g = new NameGenerator()
        .UsingProvider(x => x
            .WithVowels("ae")
            .WithConsonants("strmnl"));     
```
This example will create names like:
```
Lena
Salna
Rasse
```
See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.1:-Tailoring-Characters) for more examples on how to customize consonant positioning, vowel/consonant sequences, and grapheme weights!

## Transformations
A ```NameTransformer``` can be used to apply a transformation to a name during the generation process. This is optional and a vanilla ```NameGenerator``` will not have one by default.

Here's an example of transforming names to have specific suffixes:
```csharp
var g = new NameGenerator()
        .UsingSyllableCount(1, 2)
        .UsingTransformer(x => x
            .WithTransform(x => x.AppendSyllable("gard"))
            .WithTransform(x => x.AppendSyllable("llia")));
```
This produces names like:
```
Togard
Heshigard
Vallia
```
See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.2:-Transformations) for more information and additional examples.

## Filtering Output
You can use a ```NameFilter``` to prevent specific substrings or patterns from occurring during name generation. Filters are optional and a vanilla ```NameGenerator``` will not have one by default.

Here is an example to avoid names that start or end with certain characters:
```csharp
var g = new NameGenerator()
        .UsingFilter(x => x
            .DoNotAllowStart("x", "z")
            .DoNotAllowEnding("j", "p", "q"));
```
A ```NameGenerator``` using this filter will _not_ produce names like "Xula" or "Tesoj".

See the [wiki](https://github.com/kesac/Syllabore/wiki/Guide-1.3:-Filtering-Output) for more information and additional examples.

## Putting It All Together
Here is a more complicated name generator that could be suitable for naming cities:
```csharp
var g = new NameGenerator()
        .UsingProvider(p => p
            .WithVowels("aeoy")
            .WithLeadingConsonants("vstlr") // Only used to start a syllable
            .WithTrailingConsonants("zrt")  // Only used to end a syllable
            .WithVowelSequences("ey", "ay", "oy"))
        .UsingTransformer(m => m
            .Select(1).Chance(0.99) // 99% chance to choose 1 transform
            .WithTransform(x => x.ReplaceSyllable(0, "Gran"))
            .WithTransform(x => x.ReplaceSyllable(0, "Bri"))
            .WithTransform(x => x.InsertSyllable(0, "Deu").AppendSyllable("gard")).Weight(2)
            .WithTransform(x => x.When(-2, "[aeoyAEOY]$").ReplaceSyllable(-1, "opolis"))
            .WithTransform(x => x.When(-2, "[^aeoyAEOY]$").ReplaceSyllable(-1, "polis")))
        .UsingFilter(v => v
            .DoNotAllow("yv", "yt", "zs")
            .DoNotAllowPattern(
                @".{12,}",
                @"(\w)\1\1",              // Prevents any letter from occuring three times in a row
                @".*([y|Y]).*([y|Y]).*",  // Prevents double y
                @".*([z|Z]).*([z|Z]).*")) // Prevents double z
        .UsingSyllableCount(2, 4);
```
This example would create names like:
```
Resepolis
Varosy
Grantero
```

Check out the [wiki](https://github.com/kesac/Syllabore/wiki) for more guides!

## Serialization
The easiest way to preserve name generator settings is to just serialize a ```NameGenerator``` object into a json file. You can use the ```NameGeneratorSerializer``` class for this purpose which has a method of dealing with polymorphic deserialization:

```csharp
var g = new NameGenerator();
var s = new NameGeneratorSerializer();

// Write the name generator to disk
s.Serialize(g, "name-generator.json");
```
Then when you're ready, you can load from the json file you created earlier:
```csharp
var generator = s.Deserialize("name-generator.json");
Console.WriteLine(generator.Next());
```

## Installation
Syllabore is available as a NuGet package. You can install it from your [NuGet package manager in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

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
