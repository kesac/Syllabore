# Overview

<figure><img src=".gitbook/assets/68747470733a2f2f692e696d6775722e636f6d2f5939386f4e6c692e706e67.png" alt=""><figcaption></figcaption></figure>

### What is Syllabore?

* Syllabore is a name generator that does not use pre-made lists of names
* It is free and [open-source](https://github.com/kesac/syllabore)

### Why should I use Syllabore?

You will find it useful if:

* You do not want to randomly select names from pre-defined lists
* You want a name generator that can be used 100% offline
* You want a name generator that can be embedded into a .NET app or game

_See the_ [_starting out guide_ ](starting-out.md)_if you've already added Syllabore to your project._

### How do I add Syllabore to my project?

Syllabore is a .NET class library and works inside other .NET applications and games. It can be added to your project through a couple ways:

* In Visual Code, use the **NuGet: Add NuGet Package** command available through Microsoft's **C# Dev Kit** extension.
* In Visual Studio, search for "Syllabore" using the [NuGet package manager in Visual Studio](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio).&#x20;
* In Visual Studio, you can also run the following command in your NuGet package manager console:

```
Install-Package Syllabore
```

<details>

<summary>How do I use Syllabore with Godot?</summary>

Make sure you are using the .NET version of [Godot](https://godotengine.org/) that supports C#. There are a couple ways to import Syllabore into a Godot game:

* Open your Godot project in Visual Studio and add the Syllabore NuGet package through the [package manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio)
* Or open a command line, `cd` into your Godot project directory, and use the following command:

```
dotnet add package Syllabore
```

</details>

### What is Syllabore compatible with?

Syllabore is a [.NET Standard](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0) 2.0 class library and is compatible with applications using:

* .NET or .NET Core 2.0 to 8.0 inclusive
* .NET Framework 4.6.1 to 4.8.1 inclusive
* [Mono](https://www.mono-project.com/) 5.4 and 6.4
* [Godot 4](https://godotengine.org/download/windows/) (Using the .NET edition of the engine)
* [Unity Engine](https://unity.com/products/unity-engine)
* [MonoGame](https://www.monogame.net/)
