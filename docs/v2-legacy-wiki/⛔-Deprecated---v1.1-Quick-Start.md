_**This page only applies to version 1.1 of Syllabore. The latest version is 2.x.**_

***

## Quick Start
A generator is setup to work without any additional configuration. Just instantiate ```NameGenerator``` as-is and start calling ``Next()``:
```csharp
var g = new NameGenerator();

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(g.Next());
}

```
A default ```NameGenerator``` uses ```DefaultSyllableProvider``` for syllable generation, will not have a mutation step, and will not have a name validator.

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
