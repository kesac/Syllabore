# Syllabore
A boring name generator based on constructing syllables and randomly joining them.

## Basic Usage
```csharp
public static void Main(string[] args)
{
    var provider = new BasicSyllableProvider(); // provider of consonant and vowel combinations
    var names = new Syllabore(provider);
    
    for(int i = 0; i < 100; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```

## Constraining Names
```csharp
public static void Main(string[] args)
{
    var provider = new BasicSyllableProvider();
    var validator = new BasicNameValidator(); // optional, for constraining letter combinations
    var names = new Syllabore(provider, validator);
    
    for(int i = 0; i < 100; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```
## Sample Output with Basic Model and Validator
```
Naci
Xogud
Beqae
Crovo
Garu
Lultu
Laibu
Glowia
Goscuc
Tevu
Zimvu
Druhhest
Sumae
Vumnih
Gefa
Duvu
Qiclou
Najost
Lidfo
Godrest
Bebi
Zaprey
Thopea
Wiqa
Clunust
Jodo
Jita
```
## Changing Length of Generated Names
```csharp
public static void Main(string[] args)
{
    var provider = new BasicSyllableProvider();
    var validator = new BasicNameValidator();
    var names = new Syllabore(provider, validator);

    names.MinimumSyllables = 1;
    names.MaximumSyllables = 5;
    
    for(int i = 0; i < 100; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```



# License

MIT License

Copyright (c) 2019 Kevin Sacro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

