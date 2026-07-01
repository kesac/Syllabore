# Spaceship Names

```csharp
// This example uses two generators and a formatter
// Calls to formatter.Next() will generate names like:
// VNH Mousiavium, NSH Ratirus, MNL Rousiades, etc.

var prefixes = new NameGenerator()
    .Any(x => x
    .First(x => x
        .Add("SHMLAMN").Weight(5)
        .Add("UVX").Weight(2)))
    .Filter(@"(\w)\1\1")
    .SetSize(3);

var ships = new NameGenerator()
    .Any(x => x
    .First(x => x
        .Add("rstlmn").Weight(4)
        .Add("cdgp").Weight(2))
    .Middle(x => x
        .Add("aoi")
        .Cluster("ei", "ia", "ou", "eu")))
    .Transform(new TransformSet()
        .RandomlySelect(1)
        .Add(x => x.Replace(-1, "des"))
        .Add(x => x.Replace(-1, "rus"))
        .Add(x => x.Replace(-1, "vium")))
    .Filter(@"(\w)\1") 
    .SetSize(3);

var formatter = new NameFormatter("{prefix} {name}")
    .Define("prefix", prefixes, NameFormat.UpperCase)
    .Define("name", ships);
```

