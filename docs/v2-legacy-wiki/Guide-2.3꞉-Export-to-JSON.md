## Serialization
The easiest way to preserve name generator settings is to just serialize a ```NameGenerator``` object into a json file. You can use the ```NameGeneratorSerializer``` class for this purpose which has a method of dealing with polymorphic deserialization:

```csharp
var g = new NameGenerator();
var s = new NameGeneratorSerializer();

// Write the name generator to disk
s.Serialize(g, "name-generator.json");
```
Then when you're ready, you can load from the json file you created earlier:
```csharp
var generator = s.Deserialize("name-generator.json");
Console.WriteLine(generator.Next());
```