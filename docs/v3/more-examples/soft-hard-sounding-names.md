# Soft/Hard-Sounding Names

```csharp
// Generates names like: Lelia, Yannomo, Lammola
var names = new NameGenerator()
    .Start(x => x
        .First(x => x
            .Add("lmny").Weight(8)
            .Add("wr").Weight(2)
            .Add("s"))
        .Middle(x => x
            .Add("aeo").Weight(4)
            .Add("u")
            .Cluster("ia", "oe", "oi")))
    .Inner(x => x.CopyStart()
        .First(x => x
            .Cluster("mm", "nn", "mn", "ll")))
    .End(x => x.CopyStart()
        .Last(x => x
            .Add("smn")
            .Cluster("sh", "th"))
            .Chance(0.20))
    .SetSize(2, 3);
```

