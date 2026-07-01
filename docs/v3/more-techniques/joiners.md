# Generator Pools

Syllabore provides the convenience class [generatorpool-less-than-t-greater-than.md](../class-docs/generatorpool-less-than-t-greater-than.md "mention") for combining multiple name generators together.

Consider the following generator pool that creates both short and long names using different symbols:

```csharp
var shortnames = new NameGenerator()
    .Any(x => x
        .First("str")
        .Middle("ou"))
    .SetSize(2);

var longnames = new NameGenerator()
    .Start(x => x
        .First("lmn")
        .Middle("ae'") // Note the apostrophe
        .Last("zs"))
    .Inner(x => x
        .CopyStart())
    .End(x => x
        .Middle("aeio"))
    .SetSize(3);

var pool = new GeneratorPool<string>()
    .Add(shortnames)
    .Add(longnames);
```

Calls to `pool.Next()` generates names like:

```
Toru
M'slazi
Ruso
Nesmezo
```

