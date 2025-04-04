## Name Formatters
A ```NameFormatter``` is a special kind of generator for modeling multi-part names. A formatter can combine **more than one** generator to generate names.

## Example Formatter

Let's say you want to generate names that have two parts:
 * A given name
 * A family name

Let's also say you create the following generators for each part:
```csharp
var givenNames = new NameGenerator("ae","lmn");

var familyNames = new NameGenerator()
    .UsingSyllables(x => x
        .WithVowels("aeou")
        .WithLeadingConsonants("strlmn"))
    .UsingTransform(0.5, x => x.AppendSyllable("dar"))
    .UsingSyllableCount(3);
```

To combine the two generators, use a ```NameFormatter``` like so:
```csharp
var f = new NameFormatter("{firstname} {lastname}")
    .UsingGenerator("firstname", givenNames)
    .UsingGenerator("lastname", familyNames);
```

Calls to ```Next()``` on the example ```NameFormatter``` will produce names like:
```
Nene Toremo
Mana Salasodar
Lena Lunara
```