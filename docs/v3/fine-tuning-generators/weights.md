# Weights

### Adjusting weights of symbols

Sometimes certain symbols need to appear more often than others. You can control the frequency of a symbol by setting its _weight_.

The higher the weight, the more often the symbol will show up in generated names. Consider the following example:&#x20;

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First(x => x
            .Add("lmnr").Weight(5)  // Appears more frequently
            .Add("kgpb").Weight(2)) // Appears less frequently
        .Middle(x => x
            .Add("aei").Weight(4)
            .Add("ou").Weight(1)))
    .SetSize(2, 3);
```

<details>

<summary>See non-fluent version</summary>

```csharp
var firstSymbols = new SymbolGenerator()
    .Add("lmnr").Weight(5)
    .Add("kgpb").Weight(2);

var middleSymbols = new SymbolGenerator()
    .Add("aei").Weight(4)
    .Add("ou").Weight(1);

var syllableGenerator = new SyllableGenerator()
    .Add(SymbolPosition.First, firstSymbols)
    .Add(SymbolPosition.Middle, middleSymbols);

var names = new NameGenerator();
names.SetSyllables(SyllablePosition.Any, syllableGenerator);
names.SetSize(2, 3);
```

</details>

Calls to `names.Next()`  will generate names like:

```
Lika
Ranumi
Marile
Maloba
```

#### In the example above:

* The vowels `a` `e` `i` will appear 4x more likely than `o` `u`
* The consonants `l` `m` `n` `r` will appear 2.5x more likely than the letters `k` `g` `p` `b` because $$\frac{5\space weight}{2\space weight} = 2.5$$

{% hint style="info" %}
The default weight of a symbol is 1.
{% endhint %}

### Adjusting weights of clusters

You can set the weight of clusters the same way as you would for regular symbols. Consider the following generator:

```csharp
var names = new NameGenerator()
    .Any(x => x
        .First(x => x
            .Add("mnlr").Weight(1)
            .Cluster("th", "sh", "ch").Weight(4))
        .Middle(x => x
            .Add("aeiou").Weight(1)
            .Cluster("ia", "ae", "ei").Weight(4)))
    .SetSize(2, 3);
```

<details>

<summary>See non-fluent version</summary>

<pre class="language-csharp"><code class="lang-csharp">var firstSymbols = new SymbolGenerator()
    .Add("mnlr").Weight(1)
    .Cluster("th", "sh", "ch").Weight(4);

var middleSymbols = new SymbolGenerator()
    .Add("aeiou").Weight(1)
    .Cluster("ia", "ae", "ei").Weight(4);

var syllableGenerator = new SyllableGenerator()
    .Add(SymbolPosition.First, firstSymbols)
    .Add(SymbolPosition.Middle, middleSymbols);

var names = new NameGenerator()
<strong>    .SetSyllables(SyllablePosition.Any, syllableGenerator)
</strong>    .SetSize(2, 3);
</code></pre>

</details>

In this example, all clusters will show up 4x more likely than normal symbols. Calls to `names.Next()` will generate names like:

```
Thiashaena
Nilia
Sheichi
```
