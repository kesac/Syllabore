# Syllabore
[![Nuget](https://img.shields.io/nuget/v/Syllabore)](https://www.nuget.org/packages/Syllabore/)

Syllabore is a fantasy name generator and class library, and **does not use pre-made lists of names**. Name generation is accomplished by generating syllables from vowel-consonant pools and sequencing them into names. 

### [See the wiki for more information.](https://github.com/kesac/Syllabore/wiki)

## Quick Start
A generator is setup to work without any additional configuration. Just instantiate ```NameGenerator``` as-is and start calling ``Next()``:
```csharp
var g = new NameGenerator();

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(g.Next());
}

```
A default ```NameGenerator``` uses [all consonants and vowels in the English language](https://github.com/kesac/Syllabore/wiki/Defaults) for syllable generation and will not use mutations step or validators. 

## Customizing Name Generation
You can customize a name generator programmatically. Here is a basic three-syllable name generator:
```csharp
var g = new NameGenerator()
    .UsingProvider(x => x
        .WithLeadingConsonants("str")
        .WithVowels("ae"))
    .LimitSyllableCount(3);
```
This example would create names like:
```
Tetara
Resata
Resere
Sasata
```

Here is a more complicated name generator that could be suitable for naming cities:
```csharp
var g = new NameGenerator()
    .UsingProvider(p => p
        .WithVowels("aeoy")
        .WithLeadingConsonants("vstlr")
        .WithTrailingConsonants("zrt")
        .WithVowelSequences("ey", "ay", "oy"))
    .UsingMutator(m => m
        .WithMutation(x => { x.Syllables[0] = "gran"; })
        .WithMutation(x => x.Syllables.Add("opolis")).When(x => x.EndsWithConsonant())
        .WithMutation(x => x.Syllables.Add("polis")).When(x => x.EndsWithVowel())
        .WithMutationCount(1))
    .UsingValidator(v => v
        .DoNotAllowPattern(
            @"(\w)\1\1",             // no letters three times in a row
            @".*([y|Y]).*([y|Y]).*", // two y's in same name
            @".*([z|Z]).*([z|Z]).*", // two z's in same name
            @"(zs)",                 // this just looks weird
            @"(y[v|t])"))            // this also looks weird 
    .LimitMutationChance(0.50)
    .LimitSyllableCount(2, 3);
```
This example would create names like:
```
Resepolis
Varosy
Sola 
Grantero
```

## Mutators and Creating Variations
You can use Syllabore to produce variations of a specific name by accessing mutators directly. Here is a quick example of producing variations:
```csharp
var g = new NameGenerator();

for(int i = 0; i < 3; i++)
{
    var name = g.NextName();
    Console.WriteLine(name);

    for (int j = 0; j < 4; j++)
    {
        // Variations will be different to the original
        // in that one syllable is replaced with a new
        // randomly generated syllable
        var variation = g.NextVariation(name);
        Console.WriteLine(variation);
    }
}
```
In this example, the ```NameGenerator``` defaults to using ```DefaultNameMutator``` for its mutator. Mutation will not occur on calls to ```NextName()``` because a default ```NameGenerator``` has a mutation chance to 0%. You can increase the percentage or call ```NextVariation()``` directly to create a variation of a name.

Here is another example of configuring the mutation step of a ```NameGenerator```:
```csharp
var g = new NameGenerator()
    .UsingMutator(new VowelMutator())
    .LimitMutationChance(0.25);
```
In this example, the name generator has been given a custom mutation step in which one vowel of one syllable in a name is changed to different vowel. This is set to occur 25% of the time whenever ```Next()``` is called.


## Capturing NameGenerator settings in a file <sup>1</sup>
The static methods ```Save()``` and ```Load()``` of the ```ConfigurationFile``` class let you serialize and deserialize any instance of ```NameGenerator```.

```csharp
var g = new NameGenerator();
ConfigurationFile.Save(g, "settings.json");

var deserialized = ConfigurationFile.Load("settings.json");
```
<sup>1</sup>*As of v1.1, ```Mutations``` defined for a NameGenerator cannot be serialized. This is because the current implementation of ```NameMutators``` use lambdas.*

# Installation
Syllabore is available as a NuGet package. You can install it from your [NuGet package manager in Visual Studio](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

# License

MIT License

Copyright (c) 2019-2021 Kevin Sacro

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

