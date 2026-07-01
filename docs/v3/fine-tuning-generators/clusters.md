# Clusters

A symbol doesn't have to be a single character. Symbols with more than one character in them are called symbol clusters or just _clusters_ for short.

Clusters can be added using the `Cluster()` method. Consider the following generator:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First(x => x
            .Add("str")
            .Cluster("sh", "th")) // Clusters must be separated with commas
        .Middle(x => x
            .Add("aeo")
            .Cluster("ou")));
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("str")
    .Cluster("sh", "th"); // Add consonant clusters
    
var vowels = new SymbolGenerator("aeo")
    .Cluster("ou"); // Add vowel clusters
    
var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllables);
```

</details>

In this example, the generator is given the following rules:

* Choose from 3 symbols and 2 clusters (`sh`,`th`) for the first position of _any_ syllable
* Choose from 3 symbols and 1 cluster (`ou`) for the middle position of _any_ syllable
* Don't use any symbol or cluster for the last position of a syllable

Calls to `names.Next()` will generate names like:

```
Sashara
Rousa
Tethorou
```
