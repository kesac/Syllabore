# Generator Serialization

When serializing a [NameGenerator](../class-docs/namegenerator.md) to a json file, use the convenience class [NameGeneratorSerializer](../class-docs/namegeneratorserializer.md). The convenience class is found in the `Syllabore.Json` namespace.

Consider the following name generator:

<pre class="language-csharp"><code class="lang-csharp"><strong>var names = new NameGenerator()
</strong>    .Any(x => x
        .First(x => x.Add("bçƢ").Cluster("dr"))
        .Middle(x => x.Add("aieou").Cluster("ey"))
        .Last(x => x.Add("ヅ🂅🙂").Cluster("mn")))
    .Filter("zzzy", "abcd")
    .Transform(new TransformSet()
        .Chance(0.5)
        .RandomlySelect(2)
        .Add(x => x.Append("tar"))
        .Add(x => x.Insert(0, "arc"))
        .Add(x => x.Replace(0, "neo")))
    .SetSize(3);
</code></pre>

To save this to a file, create a serializer:

```csharp
var serializer = new NameGeneratorSerializer();
serializer.Serialize(names, "names.json");
```

To load a name generator from a json file:

```csharp
var serializer = new NameGeneratorSerializer();
var names = serializer.Deserialize("names.json");
```

{% hint style="info" %}
_NameGenerator_ has properties with interfaces as their types. Vanilla .NET deserialization won't work without some advanced setup. This convenience class does the setup for you.
{% endhint %}
