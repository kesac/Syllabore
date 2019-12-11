# Syllabore
A boring name generator based on constructing syllables and randomly joining them.

## Basic Usage
```csharp
public static void Main(string[] args)
{
    ISyllableModel model = new BasicSyllableModel(); // provider of consonants, vowels, and codas
    Syllabore names = new Syllabore(model);
    
    for(int i = 0; i < 50; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```

## Constraining Names
```csharp
public static void Main(string[] args)
{
    ISyllableModel model = new BasicSyllableModel();
    INameValidator validator = new BasicNameValidator(); // optional, for constraining letter combinations
    Syllabore names = new Syllabore(model, validator);
    
    for(int i = 0; i < 50; i++)
    {
        System.Console.WriteLine(names.Next());
    }
}
```

## Changing Length of Names
```csharp
public static void Main(string[] args)
{
    ISyllableModel model = new BasicSyllableModel();
    INameValidator validator = new BasicNameValidator();

    Syllabore names = new Syllabore(model, validator);

    names.MinimumSyllables = 1;
    names.MaximumSyllables = 5;
    
    for(int i = 0; i < 50; i++)
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

