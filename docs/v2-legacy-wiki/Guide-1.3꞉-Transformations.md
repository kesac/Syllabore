## Transformations
In Syllabore, a ```Transform``` is a mechanism for changing a source name into a new, modified name. Transforming names is useful for adding some determinism in name generation or for creating iterations on an established name.

## Basic Transforms
Call ```UsingTransform()``` on a ```NameGenerator``` to specify one or more transformations.

Here's an example ```NameGenerator``` that forces a specific prefix and suffix onto names:
```csharp
var g = new NameGenerator()
        .UsingTransform(x => x
            .ReplaceSyllable(0, "zo") // Replace the first syllable
            .AppendSyllable("ri"));   // Adds a new syllable to end of name
```
Calling ```Next()``` produces names like:
```
Zocari
Zoshari
Zojiri
```

Supply a ```double``` when calling ```UsingTransform()``` if you do not want your ```Transform``` to be applied 100% of the time. The ```double``` must be a value between 0.0 and 1.0.

Here's the same example as above except that the transform is only applied 50% of the time:
```csharp
var g = new NameGenerator()
        .UsingTransform(0.5, x => x
            .ReplaceSyllable(0, "zo") 
            .AppendSyllable("ri"));   
```

### Tip❕
 * Transformations are applied after all syllables of a name are generated and before the full name is validated against filters
 * If you use name filters, be careful not to transform names into ones that can never pass your own filters

## Randomized Transforms
Supply a ```TransformSet``` when calling ```UsingTransform()``` to introduce elements of randomization in your transformations.

Here's an example ```NameGenerator``` that forces a specific prefix or suffix, but never both:
```csharp
var g = new NameGenerator()
        .UsingTransform(new TransformSet()
            .WithTransform(x => x.ReplaceSyllable(0, "sa"))
            .WithTransform(x => x.AppendSyllable("na"))
            .RandomlySelect(1)); // Limit transforms to 1 per name
```
Calling ```Next()``` produces names like:
```
Flona
Sabi
Hisina
```
If you don't call ```RandomlySelect(int)``` on a ```TransformSet```, every ```Transform``` will be applied in the order they were defined.

## Weighted Random Transforms
When using a ```TransformSet```, you have the option to specify a weight for each ```Transform```. Weights influence how often a ```Transform``` is chosen over others. By default, every ```Transform``` has a weight of 1 and has an equal chance of being randomly selected.

Here's an example ```NameGenerator``` that:
 * Forces a specific prefix or suffix, but never both
 * Uses the prefix 70% of the time and the suffix 30% of the time
```csharp
var g = new NameGenerator()
        .UsingTransform(new TransformSet()
            .WithTransform(x => x.ReplaceSyllable(0, "te")).Weight(7)
            .WithTransform(x => x.AppendSyllable("re")).Weight(3)
            .RandomlySelect(1));
```
Weights only come into play if a call to ```RandomlySelect(int)``` was made before names are generated.


## Conditional Transforms
When defining a ```Transform```, you can provide an optional condition of when it should be applied to a name. The condition must be in the form of a regular expression.

Here's an example ```NameGenerator``` that adds a specific suffix if the first syllable of a name contains an ```s```:
```csharp
var g = new NameGenerator()
        .UsingTransform(x => x
            .When(0, "s").AppendSyllable("la"));
```

## Unserializable Transforms
A ```Transform``` that only appends, inserts, or replaces syllables can be serialized. This is very useful when you intend to export the settings of a  ```NameGenerator``` as a JSON file to import later.

If you do not need to serialize your generators, you can perform more complicated transformations using the method ```ExecuteUnserializableAction(Action<Name>)```.

Here is an example of a transform that swaps the beginning and ending syllables:

```csharp
var g = new NameGenerator()
    .UsingTransform(t => t
        .ExecuteUnserializableAction(name =>
        {
            var lastIndex = name.Syllables.Count - 1;
            var buffer = name.Syllables[0];

            name.Syllables[0] = name.Syllables[lastIndex];
            name.Syllables[lastIndex] = buffer;
        }));
```