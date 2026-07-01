# Fantasy Names

```csharp
// Generates names like Terolis, Kuregar, Serogar, Tecetia, Meruria
var names = new NameGenerator()
    .Any(x => x
        .First(x => x
            .Add("lmnstr").Weight(4)
            .Add("kc").Weight(2)
            .Add("yz"))
        .Middle(x => x
            .Add("e").Weight(4)
            .Add("ai").Weight(2)
            .Add("uo")));

names.SetSize(2);

names.Transform(new TransformSet()
    .RandomlySelect(1)
    .Add(x => x.Append("tia"))
    .Add(x => x.Append("ria"))
    .Add(x => x.Append("lis"))
    .Add(x => x.Append("gar")));
```

