# Transforms

In Syllabore, a _transform_ is any action that changes a name.&#x20;

### Using a basic transform

Consider the following generator that makes all names share the same suffix `-nia`:

```csharp
var names = new NameGenerator()
    .Any(x => x                       // For all syllables...
        .First("lmnstr")              // Use these consonants
        .Middle("aeiou"))             // And these vowels
    .Transform(x => x.Append("nia"))  // Then add this suffix to the final name
    .SetSize(2);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("lmnstr");
var vowels = new SymbolGenerator("aeiou");

var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var step = new TransformStep(TransformStepType.AppendSyllable, "nia");
var transform = new Transform().AddStep(step);

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllables)
    .SetTransform(transform)
    .SetSize(2);
```

</details>

<details>

<summary>See JSON version</summary>

```json
{
    "any": ["lmnstr", "aeiou"],
    "transforms": ["append(nia)"],
    "size": 2
}
```

</details>

Calls to `names.Next()` will produce names like:

```
Surunia
Timania
Lisonia
```

### Using a multistep transform

A _transform_ is made up of _transform steps._ Steps are actions like:

* Inserting, replacing, or deleting a syllable
* Replacing a syllable or a substring

You are are allowed to have more than one step in a _transform_:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("lmnstr")
        .Middle("aeiou"))
    .Transform(x => x
        .Insert(0, "za") // First add "za" to the beginning
        .Append("nia"))  // Then add "nia" to the end
    .SetSize(2);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("lmnstr");
var vowels = new SymbolGenerator("aeiou");

var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var step1 = new TransformStep(TransformStepType.InsertSyllable, "0", "za");
var step2 = new TransformStep(TransformStepType.AppendSyllable, "nia");

var transform = new Transform()
    .AddStep(step1)
    .AddStep(step2);

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllables)
    .SetTransform(transform)
    .SetSize(2);
```

</details>

<details>

<summary>See JSON version</summary>

```json
{
    "any": ["lmnstr", "aeiou"],
    "transforms": [
        ["insert(0,za)", "append(nia)"]
    ],
    "size": 2
}
```

</details>

This generator creates names like:

```
Zasurania
Zatamunia
Zarutonia
```

### Using a probabilistic transform

You can choose to add randomization to your transforms by using a `TransformSet`. Consider this generator:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("lmnstr")
        .Middle("aeiou"))
    .Transform(new TransformSet()
        .RandomlySelect(1)               // Only perform one transform
        .Add(x => x.Replace(-1, "des"))  // "-1" targets the end syllable
        .Add(x => x.Replace(-1, "rus"))
        .Add(x => x.Replace(-1, "vium")))
    .SetSize(3);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("lmnstr");
var vowels = new SymbolGenerator("aeiou");

var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var transform1 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "des"));

var transform2 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "rus"));
 
var transform3 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "vium"));

var transformSet = new TransformSet()
    .RandomlySelect(1)
    .Add(transform1)
    .Add(transform2)
    .Add(transform3);

var names = new NameGenerator();
names.SetSyllables(SyllablePosition.Any, syllables);
names.SetTransform(transformSet);
names.SetSize(3);
```

</details>

> [!WARNING]
> There is currently no JSON equivalent for this example. The simplified `NameGeneratorConfig` format always applies every eligible transform in a `transforms` list and has no way to express `RandomlySelect()`.

This generator replaces the last syllable with one of the suffixes `-des` `-rus` or `-vium`. The names that come out of this generator look like this:

```
Sunerus
Lasuvium
Nerades
```

### Chancing a transform

Finally, when using a `TransformSet`, you can also specify a probability of any transform being used through the `Chance()` method.

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("lmnstr")
        .Middle("aeiou"))
    .Transform(new TransformSet()
        .Chance(0.5)                     // Only use a transform 50% of the time
        .RandomlySelect(1)               // Only perform one transform
        .Add(x => x.Replace(-1, "des"))
        .Add(x => x.Replace(-1, "rus"))
        .Add(x => x.Replace(-1, "vium")))
    .SetSize(3);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("lmnstr");
var vowels = new SymbolGenerator("aeiou");

var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var transform1 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "des"));

var transform2 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "rus"));
 
var transform3 = new Transform()
    .AddStep(new TransformStep(TransformStepType.ReplaceSyllable, "-1", "vium"));

var transformSet = new TransformSet()
    .Chance(0.5)
    .RandomlySelect(1)
    .Add(transform1)
    .Add(transform2)
    .Add(transform3);

var names = new NameGenerator();
names.SetSyllables(SyllablePosition.Any, syllables);
names.SetTransform(transformSet);
names.SetSize(3);
```

</details>

> [!WARNING]
> There is currently no JSON equivalent for this example. The simplified `NameGeneratorConfig` format has no way to express `TransformSet.Chance()` or `RandomlySelect()`.

This results in names like:

```
Rumiri
Tetades
Semate
```
