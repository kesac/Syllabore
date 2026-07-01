# Chancing

By default, every syllable's symbol position (_first_, _middle_, and _last_) has a 100% chance of being included as long as there are symbols to use for that position.&#x20;

You can choose to change this probability to add more variety to your names. Consider the following generator:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("lmp")
        .Middle("aei")
        .Last("rst").Chance(0.5))  // 50% chance of using last position
    .SetSize(2, 3);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var firstSymbols = new SymbolGenerator("lmp");
var middleSymbols = new SymbolGenerator("aei");
var lastSymbols = new SymbolGenerator("rst");

var syllableGenerator = new SyllableGenerator()
    .Add(SymbolPosition.First, firstSymbols)
    .Add(SymbolPosition.Middle, middleSymbols)
    .Add(SymbolPosition.Last, lastSymbols)
    .SetChance(SymbolPosition.Last, 0.5);

var names = new NameGenerator();
names.SetSyllables(SyllablePosition.Any, syllableGenerator);
names.SetSize(2, 3);
```

</details>

Calls to `names.Next()`  will generate names like:

```
Matmali
Lapis
Parpite
```

The `Chance()` method affects the last modified position and takes a value between 0.0 and 1.0 inclusive:

* 0.0 means the position should never have a symbol appear (0%)
* 1.0 means the position should always have a symbol appear (100%)
* Values in between represent a proportional percentage between 0% and 100%
