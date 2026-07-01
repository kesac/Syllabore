# Starting Out

See the [installation guide](./) if you're looking to add Syllabore to your project for the first time.

If it's already added to your project, make sure the following directive near the top of the class file you're using generate names:

```csharp
using Syllabore;
```

## Generating simple names

Use the [NameGenerator](class-docs/namegenerator.md) class to generate names and call `Next()` to get new names. The following example creates a single name and prints it to the console:

```csharp
var names = new NameGenerator("str", "aeo");
Console.WriteLine(names.Next());
```

Each call to `names.Next()` will return names like:

```
Sasara
Rosa
Tetoro
```

To understand how generation works, continue reading below. If you want better control of generation, jump to the fine-tuning techniques like [positioning](fine-tuning-generators/positioning.md), [weights](fine-tuning-generators/weights.md), [transforms](fine-tuning-generators/transforms.md), or [filtering](fine-tuning-generators/filtering.md).&#x20;

## Understanding symbols and syllables

Names are made up of _syllables_ which are in turn made up of _symbols_. Here's how the word `wonderful` can be broken down into symbols and syllables:

<div align="left"><figure><img src=".gitbook/assets/image (5).png" alt="" width="375"><figcaption></figcaption></figure></div>

In Syllabore, names have three _syllable positions:_

* The **starting syllable**
* The **ending syllable**
* The **inner syllable**, which is always positioned between the starting and ending syllables

Inside a syllable, there are _symbol positions_:

* The **first symbol**
* The **last symbol**
* The **middle symbol,** which is always positioned in the "center" and usually a vowel

For example, here are the symbol positions in the word `dog`:

<div align="left"><figure><img src=".gitbook/assets/image (6).png" alt="" width="188"><figcaption></figcaption></figure></div>

<details>

<summary>Are some <em>symbol</em> positions optional?</summary>

All symbol positions are optional, but you'll want to supply at least one symbol for a position or you'll get empty strings for syllables.

</details>

<details>

<summary>Which <em>syllable</em> positions optional?</summary>

It depends on how many syllables there are in a name. In Syllabore:

* A name that has 1 syllable only has a _starting_ syllable
* A name that has 2 syllables has a _starting_ and _ending_ syllable

</details>

<details>

<summary>How many inner syllables can there be?</summary>

If a name has more than 3 syllables then it has more than 1 _inner_ syllable.

</details>

## Configuring symbols and syllables

A [NameGenerator](class-docs/namegenerator.md) is highly configurable. This is done by customizing its underlying [SymbolGenerators](class-docs/symbolgenerator.md) and [SyllableGenerators](class-docs/syllablegenerator.md).

Consider the following name generator.

```csharp
var names = new NameGenerator("str", "aeo");
```

This generator is setup with the following rules:

* Use symbols `s` `t` `r` in the first position of syllable
* Use symbols `a` `e` `o` in the middle position of a syllable
* Don't use anything for the last position of a syllable

Another way to configure this `NameGenerator` is to create the `SymbolGenerator` for its consonants and vowels first.&#x20;

```csharp
var consonants = new SymbolGenerator("str");
var vowels = new SymbolGenerator("aeo");

var syllables = new SyllableGenerator()
    .Add(SymbolPosition.First, consonants)
    .Add(SymbolPosition.Middle, vowels);

var names = new NameGenerator()
    .SetSyllables(SyllablePosition.Any, syllables);
```

In the example above, calls to `names.Next()` will also generate names like:

```
Sasara
Rosa
Tetoro
```

## Using the fluent API

Syllabore provides a fluent interface for configuring generators. This is optional and often cuts down on the amount of setup code.

To use the fluent interface, a class file must contain the following directive near the top of the file:

```csharp
using Syllabore.Fluent;
```

This will let you construct a name generator like so:

<pre class="language-csharp"><code class="lang-csharp"><strong>var names = new NameGenerator()
</strong>        .Any(x => x
        .First("str")
        .Middle("aeo"));
</code></pre>

In this fluent example, calls to `names.Next()` will once again generate names like:

```
Sasara
Rosa
Tetoro
```

To fine-tune generators, read on to the next section which will cover techniques like [positioning](fine-tuning-generators/positioning.md), [weights](fine-tuning-generators/weights.md), [transforms](fine-tuning-generators/transforms.md), and [filtering](fine-tuning-generators/filtering.md).&#x20;
