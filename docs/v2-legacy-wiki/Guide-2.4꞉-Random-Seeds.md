## Random Seeds
> [!Note]
> This feature is only available in version 2.3.3 or higher

Syllabore uses .NET's [System.Random](https://learn.microsoft.com/en-us/dotnet/api/system.random?view=net-8.0) to simulate randomness during the generation of syllables and names. Instantiation of a ```Random``` instance is done implicitly and you don't need to worry about doing this unless you want control over the random [seed](https://learn.microsoft.com/en-us/dotnet/api/system.random.-ctor?view=net-8.0#system-random-ctor(system-int32)). 

The following table lists classes that simulate randomness, the property name of the `System.Random` instance, and the method to use when configuring a generator through fluent methods. Example code is provided further below.

| Class | Property Name | Method |
| --- | --- | --- |
| `NameGenerator`  | `Random` | `UsingRandom()` |
| `SyllableGenerator`  | `Random` | `WithRandom()` |
| `SyllableSet`  | `Random` | `WithRandom()` |
| `TransformerSet`  | `Random` | `WithRandom()` |

> [!Note]
> You don't need to provide seeds unless you want predictable output.

### Setting the seed in a ```SyllableGenerator```

```C#
var customSeed = 12345;
var syllables = new SyllableGenerator("aeiou", "strlm");
syllables.Random = new Random(customSeed);

// Or as a one-liner
var syllables = new SyllableGenerator("aeiou", "strlm").WithRandom(new Random(12345));
```

### Setting seeds in a ```NameGenerator```
```C#
// When setting the seed for NameGenerator
// remember to also set the seed of the SyllableGenerator too.
var mySeed = 12345;
var g = new NameGenerator()
    .UsingSyllables(x => x
        .WithVowels("aeiou")
        .WithConsonants("strlm")
        .WithRandom(new Random(mySeed)))
    .UsingRandom(new Random(mySeed));

// You can also access the property directly
g.Random = new Random(mySeed);
```

### Setting the seed in a ```TransformerSet```
```C#
// This transform set either adds "prefix" to
// the start of a name or "suffix" to the end of a name
var transform = new TransformSet()
    .WithTransform(x => x.InsertSyllable(0, "prefix"))
    .WithTransform(x => x.AppendSyllable("suffix"))
    .RandomlySelect(1)
    .WithRandom(new Random(12345));

// You can also access the property directly
transform.Random = new Random(12345);
```

### Setting the seed in a ```SyllableSet```

```C#
// Constructs a finite set of syllables and
// only returns syllables from that set
var s = new SyllableSet(2, 8, 2)      // 2 * 8 * 2 = 32 possible syllables
    .WithStartingSyllable("ko", "ro") // Only these 2 starting syllables will exist
    .WithEndingSyllable("re", "ke")   // Only these 2 ending syllables will exist
    .WithGenerator(x => x             // Generator for the middle syllables
        .WithVowels("ae")
        .WithLeadingConsonants("strlmn")
    .WithRandom(new Random(12345)));

// You can also access the property directly
s.Random = new Random(12345);
```