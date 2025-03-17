using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Syllabore.Tests;

[TestClass]
public class SyllableSetTest
{
    [TestMethod]
    public void SyllableSet_ConstructorWithNoArguments_ThrowsExceptionOnGeneration()
    {
        var sut = new SyllableSet();
        Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
    }

    [TestMethod]
    public void SyllableSet_ConstructorWithNoArgumentsWithAddMethod_GeneratesSyllables()
    {
        var sut = new SyllableSet();
        sut.Add("a", "b", "c");

        var result = sut.Next();
        Assert.IsTrue(result != null && result.Length > 0);
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("a", "b", "ç")]
    [DataRow("🂅", "🙂", "ヅ")]
    public void SyllableSet_ConstructorWithStringArguments_GeneratesSameArguments(params string[] syllables)
    {
        var sut = new SyllableSet(syllables);
        var detectedSyllables = new HashSet<string>();

        for (int i = 0; i < 100; i++)
        {
            detectedSyllables.Add(sut.Next());
        }

        foreach (var syllable in syllables)
        {
            Assert.IsTrue(detectedSyllables.Contains(syllable));
        }
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(8)]
    [DataRow(32)]
    public void SyllableSet_UseGeneratorWithUniqueness_GeneratesUniqueSyllables(int limit)
    {
        var generator = new SyllableGenerator()
            .Add(SymbolPosition.First, "bcdfghjklmn") //   11 symbols
            .Add(SymbolPosition.Middle, "aeiou")      // x 5  symbols
            .Add(SymbolPosition.Last, "bcdfghjklmn"); // x 11 symbols
                                                      // = 605 combinations

        var sut = new SyllableSet(generator, limit, true);

        var detectedSyllables = new HashSet<string>();
        for (int i = 0; i < 1000; i++)
        {
            detectedSyllables.Add(sut.Next());
        }

        Assert.IsTrue(detectedSyllables.Count == limit, "Only detected " + detectedSyllables.Count + " syllables");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(8)]
    [DataRow(32)]
    public void SyllableSet_UseGeneratorWithNoUniqueness_GeneratesDuplicateSyllables(int limit)
    {
        var generator = new SyllableGenerator()
            .Add(SymbolPosition.First, "a")
            .Add(SymbolPosition.Middle, "b")
            .Add(SymbolPosition.Last, "c");

        var sut = new SyllableSet(generator, limit, false);

        var detectedSyllables = new HashSet<string>();
        for (int i = 0; i < 1000; i++)
        {
            detectedSyllables.Add(sut.Next());
        }

        Assert.IsTrue(detectedSyllables.Count == 1);
    }

    [TestMethod]
    [DataRow(8)]
    [DataRow(32)]
    public void SyllableSet_NotEnoughCombinationsForUniqueness_ThrowsException(int limit)
    {
        var generator = new SyllableGenerator()
            .Add(SymbolPosition.First, "a")
            .Add(SymbolPosition.Middle, "b")
            .Add(SymbolPosition.Last, "c");

        // Only possible combination from syllable generator is "abc"
        // When uniqueness is forced and limit > 1, it should throw an exception
        // instead of getting stuck in an infinite loop

        var sut = new SyllableSet(generator, limit, true);
        Assert.ThrowsException<InvalidOperationException>(() => sut.Next());
    }

}
