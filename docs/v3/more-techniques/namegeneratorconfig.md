# Generator Configuration Files

`NameGeneratorConfig`, found in the `Syllabore.Json` namespace, lets you configure a [NameGenerator](../class-docs/namegenerator.md) from a simplified, hand-authorable json syntax instead of code. This is different from [NameGeneratorSerializer](namegeneratorserializer.md), which reads and writes a formal, lossless json format meant to be produced by the library itself rather than typed by hand.

## Loading a config

Use `Load()` to read a config from a json file on disk, or `Parse()` if you already have the json as a string. Either way, pass the resulting `NameGeneratorConfig` into a `NameGenerator` constructor:

```csharp
var config = NameGeneratorConfig.Load("names.json");
var names = new NameGenerator(config);
```

## Syllable positions

A config can define up to four keys for syllable positions: `any`, `start`, `inner`, and `end`.

* `any` behaves like the fluent `.Any()` method — it assigns the **same** syllable generator to the starting, inner, and ending positions.
* `start`, `inner`, and `end` behave like `.Start()`, `.Inner()`, and `.End()` — each sets up its own independent syllable generator.

Each key's value is an array of up to three _slots_, in order: the first, middle, and last symbol position of that syllable. Missing trailing slots (or an explicit empty string) mean that position has no symbols.

```json
{
    "start": ["st", "eo", "mn"],
    "inner": ["pl", "ia"],
    "end": ["bcdfg", "aeiou", "mn"]
}
```

You can also combine `any` with a specific override. The override completely replaces that position instead of sharing the `any` instance:

```json
{
    "any": ["bcdfg", "aeiou", "mn"],
    "end": ["bcdfg", "aeiou", "mn*3"]
}
```

In the example above, the starting and inner syllables still share the same generator from `any`, but the ending syllable gets its own generator with a heavier weight on its last symbols.

## Symbol slot tokens

A slot can be a single string, where each character becomes its own symbol:

```json
"aeiou"
```

Or an array of tokens (and optionally one chance value, [covered below](#chance)) for more control:

```json
["str", "(sh)", "(th)"]
```

### Clusters

Wrap a token in parentheses to keep it atomic instead of exploding it into individual characters. This is the json equivalent of `.Cluster()`:

```json
["str", "(sh)", "(th)"]
```

The example above uses the symbols `s` `t` `r` plus the two clusters `sh` and `th`.

### Weights

Append `*N` to a token to set its weight, the json equivalent of `.Weight()`:

```json
["aeiou*3", "y"]
```

Here, each of `a` `e` `i` `o` `u` gets a weight of 3 while `y` keeps the default weight of 1. Weights work on clusters too:

```json
["bcdfg", "(dr)*2", "(th)"]
```

### Chance

A slot's array may include a single number, its json equivalent of `.Chance()` for that symbol position:

```json
["mn", "(ng)*2", 0.5]
```

This gives the position a 50% chance of appearing in a syllable at all, independent of any other position's chance.

## Reusing definitions with `$reference`

Instead of repeating a definition, point one position at another using `"$positionName"`. This is the json equivalent of `.CopyInner()` and similar copy helpers:

```json
{
    "inner": ["mnr", "ioa"],
    "end": "$inner"
}
```

`end` will use the exact same syllable generator instance as `inner`. A reference that points to an undefined position, or that forms a cycle (e.g. `"start": "$inner", "inner": "$start"`), throws a `JsonException`.

## Size

The `size` property sets `MinimumSize` and `MaximumSize`:

```json
"size": 3
```

A single number fixes both the minimum and maximum to the same value. An array sets a range:

```json
"size": [2, 4]
```

If `size` is omitted, it defaults to a range of 2 to 3, matching `NameGenerator`'s own defaults.

## Filters

`filters` is an array of pattern strings, each becoming a `FilterConstraint`:

| Pattern | Condition |
| --- | --- |
| `/regex/` | Matches the regular expression |
| `*text*` | Contains `text` |
| `*text` | Ends with `text` |
| `text*` | Starts with `text` |
| `text` (no sigil) | Contains `text` |

```json
"filters": ["*ash*", "*ex", "xx*", "/[qxz]{2,}/", "literal"]
```

## Transforms

`transforms` is an array where each item describes one `Transform`.

A bare string is a single, unconditional step:

```json
"transforms": ["append(nia)"]
```

An item can also be an array combining several things:

* One or more step strings, applied together, in order, as part of the same transform
* One `"?<index>~<regex>"` condition string, e.g. `"?0~[aeiou]$"` — the transform only runs if the syllable at that index matches the regex (negative indices count from the end of the name, like `-1` for the last syllable)
* One chance number between `0.0` and `1.0` — the equivalent of that transform's own `Chance` property

```json
"transforms": [
    ["?0~[aeiou]$", "append(th)", 0.3]
]
```

### Step verbs

| Verb | Maps to | Arguments |
| --- | --- | --- |
| `append(text)` | Appends a new syllable | `text` |
| `insert(index,text)` | Inserts a new syllable | `index`, `text` |
| `replace(index,text)` | Replaces a syllable | `index`, `text` |
| `remove(index)` | Removes a syllable | `index` |
| `replaceAll(substring,replacement)` | Replaces a substring across every syllable | `substring`, `replacement` |

`index` can be negative to count from the end of the name, the same way it works for `Transform`'s fluent methods.

A transform can chain multiple steps in one entry, applied in order:

```json
"transforms": [
    ["insert(0,za)", "append(nia)"]
]
```

Step arguments are split on commas. Use `\,`, `\(`, `\)`, or `\\` inside an argument to escape a literal comma, parenthesis, or backslash.

> [!WARNING]
> Every transform in the `transforms` array is applied whenever its own condition and chance allow it — there is currently no way to express `TransformSet.RandomlySelect()` or an overall `TransformSet.Chance()` through this simplified format. Use [NameGeneratorSerializer](namegeneratorserializer.md) if you need those.

## Putting it all together

```json
{
    "start": ["st", "aeo", "rl"],
    "inner": [["mnr", "(th)*2"], "ioa"],
    "end": "$inner",
    "filters": ["*ss*", "/y$/"],
    "transforms": [
        "append(ia)",
        ["?0~^[st]", "replace(0,zr)", 0.4]
    ],
    "size": [2, 4]
}
```

Load it and generate names the same way as any other `NameGenerator`:

```csharp
var config = NameGeneratorConfig.Load("names.json");
var names = new NameGenerator(config);
Console.WriteLine(names.Next());
```
