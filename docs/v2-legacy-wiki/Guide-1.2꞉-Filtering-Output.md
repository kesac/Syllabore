## Basic Filtering
Each ```NameGenerator``` can be configured to prevent specific substrings or patterns from showing up in names. 

Filtering is completely optional, but is useful in avoiding awkward sounding combinations of characters.

Here is a basic example to prevent the substrings ```ist``` and ```ck``` from appearing in names:
```csharp
var g = new NameGenerator()
        .DoNotAllow("ist") // Will prevent names like "Misty"
        .DoNotAllow("ck"); // Will prevent names like "Brock"
```

## Using Regular Expressions
The ```DoNotAllow(string)``` method of ```NameGenerator``` supports formatted strings describing [regular expressions](https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference). 

Here is a basic example that prevents a specific prefix and suffix:
```csharp
var g = new NameGenerator()
        .DoNotAllow("rd$")  // Prevents "Picard", but not "Varda"
        .DoNotAllow("^ri"); // Prevents "Riker", but not "Dorian"
```

## Configuring NameFilters Directly
When you call ```DoNotAllow(string)``` on a ```NameGenerator```, you are actually configuring the internal ```NameFilter```.

You can choost to customize the ```NameFilter``` directly if needed. Here's an example of instantiating a new ```NameFilter``` and passing it onto a new ```NameGenerator```:

```csharp
var f = new NameFilter()
        .DoNotAllowStart("thr")     // Prevents "Thrond", but not "Athrun"
        .DoNotAllowSubstring("tse") // Prevents "Tsen", "Betsey", etc.
        .DoNotAllowEnding("j")      // Prevents "Kaj", but not "Javal"
        .DoNotAllow(@"(\w)\1\1");   // Prevents "Mareeen", but not "Mareen"

var g = new NameGenerator()
        .UsingFilter(f);
```
Or in a more compact way:
```csharp
var g = new NameGenerator()
        .UsingFilter(x => x
            .DoNotAllowStart("thr")
            .DoNotAllowSubstring("tse")
            .DoNotAllowEnding("j")
            .DoNotAllow(@"(\w)\1\1"));
```