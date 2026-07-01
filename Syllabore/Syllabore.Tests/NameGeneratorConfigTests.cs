using Microsoft.VisualStudio.TestTools.UnitTesting;
using Syllabore.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Syllabore.Tests
{
    /// <summary>
    /// Tests for <see cref="NameGeneratorConfig"/>, which parses the simplified,
    /// hand-authorable json syntax (as opposed to the lossless round-trip format
    /// produced by <see cref="NameGeneratorSerializer"/>).
    /// </summary>
    [TestClass]
    public class NameGeneratorConfigTests
    {
        private SymbolGenerator SymbolsOf(NameGeneratorConfig config, SyllablePosition syllablePosition, SymbolPosition symbolPosition)
        {
            var syllableGenerator = (SyllableGenerator)config.SyllableGenerators[syllablePosition];
            return syllableGenerator.SymbolGenerators[symbolPosition].Single();
        }

        [TestMethod]
        public void Config_Any_AppliesSameGeneratorToAllPositions()
        {
            var json = @"{ ""any"": [""bcdfg"", ""aeiou"", ""mn""] }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.AreEqual(3, sut.SyllableGenerators.Count);
            Assert.AreSame(sut.SyllableGenerators[SyllablePosition.Starting], sut.SyllableGenerators[SyllablePosition.Inner]);
            Assert.AreSame(sut.SyllableGenerators[SyllablePosition.Inner], sut.SyllableGenerators[SyllablePosition.Ending]);

            var first = SymbolsOf(sut, SyllablePosition.Starting, SymbolPosition.First);
            var middle = SymbolsOf(sut, SyllablePosition.Starting, SymbolPosition.Middle);
            var last = SymbolsOf(sut, SyllablePosition.Starting, SymbolPosition.Last);

            CollectionAssert.AreEquivalent(new[] { "b", "c", "d", "f", "g" }, first.Symbols.Select(s => s.Value).ToList());
            CollectionAssert.AreEquivalent(new[] { "a", "e", "i", "o", "u" }, middle.Symbols.Select(s => s.Value).ToList());
            CollectionAssert.AreEquivalent(new[] { "m", "n" }, last.Symbols.Select(s => s.Value).ToList());
            Assert.IsTrue(first.Symbols.All(s => s.Weight == 1));
        }

        [TestMethod]
        public void Config_DistinctPositions_OpenSlotHasNoSymbolsForThatPosition()
        {
            var json = @"{ ""start"": [""bcdfg"", ""aeiou"", """"] }";
            var sut = NameGeneratorConfig.Parse(json);

            var start = (SyllableGenerator)sut.SyllableGenerators[SyllablePosition.Starting];

            Assert.IsTrue(start.SymbolGenerators.ContainsKey(SymbolPosition.First));
            Assert.IsTrue(start.SymbolGenerators.ContainsKey(SymbolPosition.Middle));
            Assert.IsFalse(start.SymbolGenerators.ContainsKey(SymbolPosition.Last));
        }

        [TestMethod]
        public void Config_MissingTrailingSlots_AreTreatedAsEmpty()
        {
            // Only 2 of the 3 possible slots (first, middle) are specified.
            var json = @"{ ""inner"": [""mnr"", ""ioa""] }";
            var sut = NameGeneratorConfig.Parse(json);

            var inner = (SyllableGenerator)sut.SyllableGenerators[SyllablePosition.Inner];

            Assert.IsTrue(inner.SymbolGenerators.ContainsKey(SymbolPosition.First));
            Assert.IsTrue(inner.SymbolGenerators.ContainsKey(SymbolPosition.Middle));
            Assert.IsFalse(inner.SymbolGenerators.ContainsKey(SymbolPosition.Last));
        }

        [TestMethod]
        public void Config_WeightedLetterRun_AppliesWeightToEachExplodedLetter()
        {
            var json = @"{ ""any"": [""bcdfg"", [""aeiou*3"", ""y""], ""mn""] }";
            var sut = NameGeneratorConfig.Parse(json);

            var middle = SymbolsOf(sut, SyllablePosition.Starting, SymbolPosition.Middle);

            Assert.AreEqual(6, middle.Symbols.Count);
            Assert.IsTrue(middle.Symbols.Where(s => "aeiou".Contains(s.Value)).All(s => s.Weight == 3));
            Assert.AreEqual(1, middle.Symbols.Single(s => s.Value == "y").Weight);
        }

        [TestMethod]
        public void Config_ClusterTokens_AreKeptAtomicAndSupportWeight()
        {
            var json = @"{ ""any"": [[""bcdfg"", ""(dr)*2"", ""(th)""], ""aeiou"", ""mn""] }";
            var sut = NameGeneratorConfig.Parse(json);

            var first = SymbolsOf(sut, SyllablePosition.Starting, SymbolPosition.First);

            Assert.AreEqual(7, first.Symbols.Count); // b,c,d,f,g + dr + th
            Assert.IsTrue(first.Symbols.Any(s => s.Value == "dr" && s.Weight == 2));
            Assert.IsTrue(first.Symbols.Any(s => s.Value == "th" && s.Weight == 1));
            Assert.IsTrue("bcdfg".ToCharArray().All(c => first.Symbols.Any(s => s.Value == c.ToString() && s.Weight == 1)));
        }

        [TestMethod]
        public void Config_ChanceInSlotArray_SetsPositionChanceWithoutAffectingOtherSlots()
        {
            var json = @"{ ""inner"": ["""", ""ioa"", [""mn"", ""(ng)*2"", 0.5]] }";
            var sut = NameGeneratorConfig.Parse(json);

            var inner = (SyllableGenerator)sut.SyllableGenerators[SyllablePosition.Inner];

            Assert.AreEqual(0.5, inner.PositionChance[SymbolPosition.Last]);
            Assert.AreEqual(1.0, inner.PositionChance[SymbolPosition.Middle]); // unaffected, stays at the default

            var last = inner.SymbolGenerators[SymbolPosition.Last].Single();
            Assert.AreEqual(3, last.Symbols.Count); // m, n, ng
            Assert.IsTrue(last.Symbols.Any(s => s.Value == "ng" && s.Weight == 2));
        }

        [TestMethod]
        public void Config_DollarReference_CopiesResolvedDefinition()
        {
            var json = @"{ ""inner"": [""mnr"", ""ioa""], ""end"": ""$inner"" }";
            var sut = NameGeneratorConfig.Parse(json);

            var inner = (SyllableGenerator)sut.SyllableGenerators[SyllablePosition.Inner];
            var end = (SyllableGenerator)sut.SyllableGenerators[SyllablePosition.Ending];

            var innerFirst = inner.SymbolGenerators[SymbolPosition.First].Single().Symbols.Select(s => s.Value).ToList();
            var endFirst = end.SymbolGenerators[SymbolPosition.First].Single().Symbols.Select(s => s.Value).ToList();

            CollectionAssert.AreEquivalent(innerFirst, endFirst);
        }

        [TestMethod]
        public void Config_CircularReference_ThrowsJsonException()
        {
            var json = @"{ ""start"": ""$inner"", ""inner"": ""$start"" }";
            Assert.ThrowsException<JsonException>(() => NameGeneratorConfig.Parse(json));
        }

        [TestMethod]
        public void Config_AnyWithPositionOverride_OverrideReplacesWholePosition()
        {
            var json = @"{
                ""any"": [""bcdfg"", ""aeiou"", ""mn""],
                ""end"": [""bcdfg"", ""aeiou"", ""mn*3""]
            }";

            var sut = NameGeneratorConfig.Parse(json);

            // Starting and Inner still share the "any" instance.
            Assert.AreSame(sut.SyllableGenerators[SyllablePosition.Starting], sut.SyllableGenerators[SyllablePosition.Inner]);

            // Ending was fully replaced by its own override, so it's a different instance...
            Assert.AreNotSame(sut.SyllableGenerators[SyllablePosition.Inner], sut.SyllableGenerators[SyllablePosition.Ending]);

            // ...with the overridden content.
            var endLast = SymbolsOf(sut, SyllablePosition.Ending, SymbolPosition.Last);
            Assert.IsTrue(endLast.Symbols.All(s => s.Weight == 3));
        }

        [TestMethod]
        public void Config_NoSizeSpecified_UsesDefaults()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""] }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.AreEqual(2, sut.MinimumSize);
            Assert.AreEqual(3, sut.MaximumSize);
        }

        [TestMethod]
        public void Config_FixedSize_SetsMinimumAndMaximumToSameValue()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""], ""size"": 3 }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.AreEqual(3, sut.MinimumSize);
            Assert.AreEqual(3, sut.MaximumSize);
        }

        [TestMethod]
        public void Config_SizeRange_SetsMinimumAndMaximum()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""], ""size"": [2, 4] }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.AreEqual(2, sut.MinimumSize);
            Assert.AreEqual(4, sut.MaximumSize);
        }

        [TestMethod]
        public void Config_Filters_ParsesAllSigilVariants()
        {
            var json = @"{
                ""any"": [""b"", ""a"", ""n""],
                ""filters"": [""*ass*"", ""*ex"", ""xx*"", ""/[qxz]{2,}/"", ""literal""]
            }";

            var sut = NameGeneratorConfig.Parse(json);
            var constraints = sut.Filter.Constraints;

            Assert.AreEqual(5, constraints.Count);

            Assert.AreEqual(FilterCondition.Contains, constraints[0].Type);
            Assert.AreEqual("ass", constraints[0].Value);

            Assert.AreEqual(FilterCondition.EndsWith, constraints[1].Type);
            Assert.AreEqual("ex", constraints[1].Value);

            Assert.AreEqual(FilterCondition.StartsWith, constraints[2].Type);
            Assert.AreEqual("xx", constraints[2].Value);

            Assert.AreEqual(FilterCondition.MatchesPattern, constraints[3].Type);
            Assert.AreEqual("[qxz]{2,}", constraints[3].Value);

            Assert.AreEqual(FilterCondition.Contains, constraints[4].Type);
            Assert.AreEqual("literal", constraints[4].Value);
        }

        [TestMethod]
        public void Config_NoFiltersSpecified_FilterIsNull()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""] }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.IsNull(sut.Filter);
        }

        [TestMethod]
        public void Config_BareStringTransform_IsASingleUnconditionalStep()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""], ""transforms"": [""append(tar)""] }";
            var sut = NameGeneratorConfig.Parse(json);

            var transform = sut.Transform.Transforms.Single();

            Assert.AreEqual(1, transform.Chance);
            Assert.IsNull(transform.ConditionalRegex);
            Assert.AreEqual(1, transform.Steps.Count);
            Assert.AreEqual(TransformStepType.AppendSyllable, transform.Steps[0].Type);
            Assert.AreEqual("tar", transform.Steps[0].Arguments[0]);
        }

        [TestMethod]
        public void Config_ConditionAndChance_AreParsedCorrectly()
        {
            var json = @"{
                ""any"": [""b"", ""a"", ""n""],
                ""transforms"": [ [""?0~[aeiou]$"", ""append(th)"", 0.3] ]
            }";

            var sut = NameGeneratorConfig.Parse(json);
            var transform = sut.Transform.Transforms.Single();

            Assert.AreEqual(0, transform.ConditionalIndex);
            Assert.AreEqual("[aeiou]$", transform.ConditionalRegex);
            Assert.AreEqual(0.3, transform.Chance);
            Assert.AreEqual(1, transform.Steps.Count);
        }

        [TestMethod]
        public void Config_MultipleSteps_AreAppliedInOrder()
        {
            var json = @"{
                ""any"": [""b"", ""a"", ""n""],
                ""transforms"": [
                    [""replace(0,neo)"", ""append(tar)"", ""insert(1,arc)"", ""replaceAll(u,uu)""]
                ]
            }";

            var sut = NameGeneratorConfig.Parse(json);
            var steps = sut.Transform.Transforms.Single().Steps;

            Assert.AreEqual(4, steps.Count);

            Assert.AreEqual(TransformStepType.ReplaceSyllable, steps[0].Type);
            CollectionAssert.AreEqual(new[] { "0", "neo" }, steps[0].Arguments);

            Assert.AreEqual(TransformStepType.AppendSyllable, steps[1].Type);
            CollectionAssert.AreEqual(new[] { "tar" }, steps[1].Arguments);

            Assert.AreEqual(TransformStepType.InsertSyllable, steps[2].Type);
            CollectionAssert.AreEqual(new[] { "1", "arc" }, steps[2].Arguments);

            Assert.AreEqual(TransformStepType.ReplaceAllSubstring, steps[3].Type);
            CollectionAssert.AreEqual(new[] { "u", "uu" }, steps[3].Arguments);
        }

        [TestMethod]
        public void Config_EscapedCommaInStepArgument_IsTreatedAsLiteral()
        {
            // The json text below contains a literal backslash-backslash-comma sequence,
            // which json decodes down to a single backslash followed by a comma.
            var json = @"{ ""any"": [""b"", ""a"", ""n""], ""transforms"": [""append(th\\,or)""] }";
            var sut = NameGeneratorConfig.Parse(json);

            var step = sut.Transform.Transforms.Single().Steps.Single();

            Assert.AreEqual(TransformStepType.AppendSyllable, step.Type);
            Assert.AreEqual("th,or", step.Arguments[0]);
        }

        [TestMethod]
        public void Config_NoTransformsSpecified_TransformIsNull()
        {
            var json = @"{ ""any"": [""b"", ""a"", ""n""] }";
            var sut = NameGeneratorConfig.Parse(json);

            Assert.IsNull(sut.Transform);
        }

        [TestMethod]
        public void NameGenerator_ConstructedFromConfig_GeneratesExpectedDeterministicName()
        {
            // Each slot has exactly one symbol, so generation is deterministic
            // regardless of randomness (same trick used by NameGeneratorFluentTests).
            var json = @"{
                ""start"": [""a"", ""b"", ""c""],
                ""inner"": [""d"", ""e"", ""f""],
                ""end"": [""g"", ""h"", ""i""],
                ""size"": 3
            }";

            var config = NameGeneratorConfig.Parse(json);
            var sut = new NameGenerator(config);

            Assert.AreEqual("Abcdefghi", sut.Next());
        }

        [TestMethod]
        public void NameGenerator_ConstructedFromConfig_AppliesAllEligibleTransformsInOrder()
        {
            var json = @"{
                ""start"": [""a"", ""b"", ""c""],
                ""inner"": [""d"", ""e"", ""f""],
                ""end"": [""g"", ""h"", ""i""],
                ""size"": 3,
                ""transforms"": [ ""append(x)"", ""append(y)"" ]
            }";

            var config = NameGeneratorConfig.Parse(json);
            var sut = new NameGenerator(config);

            Assert.AreEqual("Abcdefghixy", sut.Next());
        }

        [TestMethod]
        public void NameGeneratorConfig_Load_ReadsFileFromDisk()
        {
            var json = @"{ ""any"": [""a"", ""e"", ""n""], ""size"": 2 }";
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");

            try
            {
                File.WriteAllText(path, json);
                var config = NameGeneratorConfig.Load(path);

                Assert.AreEqual(2, config.MinimumSize);
                Assert.AreEqual(2, config.MaximumSize);

                var sut = new NameGenerator(config);
                Assert.AreEqual("Aenaen", sut.Next()); // fixed size of 2 syllables, each "aen"
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
