## Installation
### [.NET apps](https://learn.microsoft.com/en-us/dotnet/core/introduction) 
Syllabore is available as a [NuGet](https://learn.microsoft.com/en-us/nuget/what-is-nuget) package. You can install it from your [NuGet package manager in Visual Studio](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio) (search for "Syllabore") or by running the following command in your NuGet package manager console:
```
Install-Package Syllabore
```

### [Godot Engine](https://godotengine.org/)
There are a couple ways to do this in [Godot](https://godotengine.org/):
- Open your Godot project in Visual Studio and add the Syllabore NuGet package through the [package manager](https://learn.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio)
- Or open a command line, `cd` into your Godot project directory, and use the following command:
```
dotnet add package Syllabore
```

## Compatibility
By design, Syllabore is a [.NET Standard](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0) 2.0 class library. This means it will be compatible with applications using:
 * .NET or .NET Core 2.0, 2.1, 2.2, 3.0, 3.1, 5.0, 6.0, 7.0, 8.0
 * .NET Framework 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8, 4.8.1
 * [Mono](https://www.mono-project.com/) 5.4, 6.4
 
Syllabore has been tested and known to work in the following game engines:
 * [Godot 4](https://godotengine.org/download/windows/) (Using the .NET edition of the engine)
 
Syllabore should also work in the following game engines, but I have not done adequate testing yet:
 * [Godot 3](https://godotengine.org/download/3.x/windows/)
 * [Unity Engine](https://unity.com/products/unity-engine)
 * [MonoGame](https://www.monogame.net/)