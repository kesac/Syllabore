[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

![](https://i.imgur.com/qUMcu2tm.png) 

## Overview
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
 1. [Filtering](#filtering)
 1. [Putting It All Together](#putting-it-all-together)
 1. [Serialization](#serialization)
 1. [Advanced Use](#advanced-use)
 1. [Installation](#installation)
 1. [License](#license)

## Quick Start
Use the ```NameGenerator``` class to generate names. Call ``Next()`` to get a new name. By default, [all consonants and vowels in the English language](https://github.com/kesac/Syllabore/wiki/Defaults) will be used in syllables. 

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
var p = new SyllableProvider()
        .WithVowels("ae")
        .WithConsonants("strmnl");

var g = new NameGenerator().UsingProvider(p);     
```
Or in a more compact way:
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
See the [wiki](https://github.com/kesac/Syllabore/wiki/v2.x-Customizing-Syllable-Providers) for more examples on customizing providers. The topics of consonant positioning, vowel/consonant sequences, and grapheme weights are not shown here.

## Transformations
A ```NameTransformer``` can be used to apply a transformation to a name during the generation process. This is optional and a vanilla ```NameGenerator``` will not have one by default.

Here's an example of transforming names to have specific suffixes:
```csharp
var t = new NameTransformer()
         .WithTransform(x => x.AppendSyllable("gard"))
         .WithTransform(x => x.AppendSyllable("llia"));

var g = new NameGenerator()
        .UsingTransformer(t)
        .UsingSyllableCount(1, 2);
```
Or in a more compact way:
```csharp
var g = new NameGenerator()
        .UsingTransformer(x => x
            .WithTransform(x => x.AppendSyllable("gard"))
            .WithTransform(x => x.AppendSyllable("llia")))
        .UsingSyllableCount(1, 2);
```
This produces names like:
```
Togard
Heshigard
Vallia
```
(In the example, you'll notice we made a call to ```UsingSyllableCount()```. This call set the minimum syllable count to 1 and maximum to 2. The default syllable count in **Syllable** is 2 for both minimum and maximum.)

## Filtering Output
You can use a ```NameFilter``` to preventing specific substrings or patterns from occurring during name generation. Filters are optional and a vanilla ```NameGenerator``` will not have one by default.

Here is an example to avoid awkward sounding consonant endings, sequences of 3 or more identical letters, and sequences of 4 or more consonants.
```csharp
var f = new NameFilter()
        .DoNotAllowEnding("j","p","q","w")
        .DoNotAllowPattern(@"(\w)\1\1")
        .DoNotAllowPattern(@"([^aeiouAEIOU])\1\1\1");

var g = new NameGenerator().UsingFilter(f);
```
Or in a more compact way:
```csharp
var g = new NameGenerator()
        .UsingFilter(x => x
            .DoNotAllowEnding("j","p","q","w")
            .DoNotAllowPattern(@"(\w)\1\1")
            .DoNotAllowPattern(@"([^aeiouAEIOU])\1\1\1"));
```
A ```NameGenerator``` using this filter will _not_ produce names like "Rukaaa" or "Tesoj".

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
Sola 
Grantero
```

Check out the [wiki](https://github.com/kesac/Syllabore/wiki) for more advanced guides!

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
## Advanced Use
[See the wiki for more information.](https://github.com/kesac/Syllabore/wiki) The wiki contains extra technical information, additional guides, etc.

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
