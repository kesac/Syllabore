# Positioning

Any name or word is made up of syllables which are in turn made up of symbols. In Syllabore, you control which symbols to use for each _syllable position_ of a name. &#x20;

<div align="left"><figure><img src="../.gitbook/assets/image (5).png" alt="" width="188"><figcaption></figcaption></figure></div>

Consider the following example:

```csharp
var names = new NameGenerator()
    .Start(x => x     // The starting syllable of a name
        .First("st")  // Leading consonants
        .Middle("eo") // Vowels
        .Last("mn"))  // Trailing consonants
    .Inner(x => x     // The "body" of a name
        .First("pl")
        .Middle("ia"))
    .End(x => x       // The ending syllable of a name
        .CopyInner()) // Use the same symbols as inner syllables
    .SetSize(3);      // Makes names 3 syllables long
```

<details>

<summary>See non-fluent version</summary>

```csharp
var startingSyllables = new SyllableGenerator()
    .Add(SymbolPosition.First, "st")
    .Add(SymbolPosition.Middle, "eo")
    .Add(SymbolPosition.Last, "mn");

var innerSyllables = new SyllableGenerator()
    .Add(SymbolPosition.First, "pl")
    .Add(SymbolPosition.Middle, "ia");

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Starting, startingSyllables)
    .SetSyllables(SyllablePosition.Inner, innerSyllables)
    .SetSyllables(SyllablePosition.Ending, innerSyllables)
    .SetSize(3);
```

</details>

<details>

<summary>See JSON version</summary>

```json
{
    "start": ["st", "eo", "mn"],
    "inner": ["pl", "ia"],
    "end": "$inner",
    "size": 3
}
```

</details>

This generator will only use 7 symbols for the _starting_ syllable of a name and then a different set of 4 symbols for the _inner_ or _ending_ syllable.

Calls to `names.Next()` will generate names like

```
Tonpali
Sonlili
Tenlipa
```

{% hint style="info" %}
The call to `SetSize(3)` forces all generated names to be exactly 3 syllables long.&#x20;
{% endhint %}
