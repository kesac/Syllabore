# Formatters

Syllabore provides a [nameformatter.md](../class-docs/nameformatter.md "mention") class for modelling names that have multiple parts and need more than one generator to create them.

Consider the following formatter that creates names in the format `firstname` `lastname`:

```csharp
var firstnames = new NameGenerator()
    .Any(x => x
        .First("srl")
        .Middle("ae"))
    .SetSize(2);

var lastnames = new NameGenerator()
    .Any(x => x
        .First("mn")
        .Middle("iou")
        .Last("dtr"))
    .SetSize(2);

var formatter = new NameFormatter("{first} {last}")
    .Define("first", firstnames)
    .Define("last", lastnames);
```

Calling `formatter.Next()` will create names like:

```
Lara Nirmot
Sesa Midnir
Rela Mudnut
```

