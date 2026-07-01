# Filtering

### Filtering symbol combos

Generators let you block symbol combinations or patterns from showing up in names.&#x20;

Consider the following generator that does not create names with two consecutive `t` symbols: &#x20;

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("st")
        .Middle("aeiou")
        .Last("st"))
    .Filter("tt");  // Prevent two t's in a row
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("st");
var vowels = new SymbolGenerator("aeiou");

var syllableGenerator = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels)
    .Add(SymbolPosition.Last, consonants);

var nameFilter = new NameFilter()
    .Add(new FilterConstraint(FilterCondition.MatchesPattern, "tt"));

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllableGenerator)
    .SetFilter(nameFilter)
    .SetSize(2, 3);
```

</details>

✅This generator creates names like:

```
Tostis
Sastus
Tessesos
```

❌This generator will never create names like:

```
Sottus
Tottis
```

### Filtering through regex

If you are comfortable using [regular expressions](https://en.wikipedia.org/wiki/Regular_expression), you can choose to use in your filter.

The following generator uses the symbols `m` `u`, but uses a filter to prevent `m` from appearing at the beginning of a name and prevents `u` from ending it:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("strlmn")
        .Middle("aeiou"))
    .Filter("^M|u$");
```

<details>

<summary>See non-fluent version</summary>

```csharp
var consonants = new SymbolGenerator("st");
var vowels = new SymbolGenerator("aeiou");

var syllableGenerator = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels)
    .Add(SymbolPosition.Last, consonants);

var nameFilter = new NameFilter()
    .Add(new FilterConstraint(FilterCondition.MatchesPattern, "^M|u$"));

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllableGenerator)
    .SetFilter(nameFilter)
    .SetSize(2, 3);
```

</details>

This generates names like:

```
Temaro
Rima
Narumi
```

### Filtering multiple things at once

Calls to the fluent method `Filter()` implicitly sets a `NameFilter` on the `NameGenerator` you are setting up. There can only be one `NameFilter` on a `NameGenerator`.

❌Don't do this if you want to filter multiple patterns:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("strlmn")
        .Middle("aeiou"))
    .Filter("^M")
    .Filter("u$")
    .Filter("le"); // Only the last filter gets applied
```

✅Do this if you want to filter multiple patterns or combos at once:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First("strlmn")
        .Middle("aeiou"))
    .Filter(x => x
        .DoNotAllowStart("m")  // All of these get applied
        .DoNotAllowRegex("u$")
        .DoNotAllowSubstring("le"));
```

This generates names like:

```
Lisone
Nara
Ronimo
```

