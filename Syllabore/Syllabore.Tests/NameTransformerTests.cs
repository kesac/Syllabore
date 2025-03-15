using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using Syllabore.Fluent;
using System.Collections.Generic;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameTransformerTests
    {
        private Name _testName;

        public NameTransformerTests()
        {
            _testName = new Name("syl", "la", "bore");
        }

        [TestMethod]
        public void Transform_NoSteps_NoChanges()
        {
            var sut = new Transform();
            Assert.AreEqual(_testName.ToString(), sut.Apply(_testName).ToString());
        }

        [TestMethod]
        [DataRow("Asyllabore", TransformStepType.InsertSyllable, "0", "a")]
        [DataRow("Sylbore", TransformStepType.RemoveSyllable, "1")]
        [DataRow("Sylbore", TransformStepType.RemoveSyllable, "-2")]
        [DataRow("Aylabore", TransformStepType.ReplaceSyllable, "0", "ay")]
        [DataRow("Syllara", TransformStepType.ReplaceSyllable, "-1", "ra")]
        [DataRow("Syllabora", TransformStepType.ReplaceAllSubstring, "bore", "bora")]
        [DataRow("Syllaborexa", TransformStepType.AppendSyllable, "xa")]
        [DataRow("Syllabore", TransformStepType.Unknown)]
        public void Transform_OneStep_StepAppears(
            string expectedResult, TransformStepType testType, params string[] testArguments)
        {
            var sut = new Transform().AddStep(new TransformStep(testType, testArguments));

            var result = sut.Apply(_testName);
            Assert.AreEqual(result.ToString(), expectedResult);
        }

        [TestMethod]
        public void Transform_ModifyDirectly_NameChanges()
        {
            var sut = new Transform().Insert(0, "prefix");
            var copy = new Name(_testName);

            Assert.AreEqual(copy.ToString(), _testName.ToString());
            sut.Modify(copy);
            Assert.AreNotEqual(copy.ToString(), _testName.ToString());

        }

        [TestMethod]
        public void Transform_ConditionalStep_ConditionAffectsResults()
        {
            // Transform should only apply if the second syllable is "la"
            var sut = new Transform()
                .When(1, "la")
                .Insert(0, "prefix");

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix"));

            // Transform should only apply if the second syllable is "lla"
            // (This should not apply changes)
            sut = new Transform()
                .When(1, "lla")
                .Insert(0, "prefix");

            result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix"));
        }

        [TestMethod]
        public void Transform_MultipleSteps_AllStepsAppear()
        {
            var sut = new Transform()
                .Insert(0, "prefix")
                .Append("suffix");

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
        }

        [TestMethod]
        public void Transform_StepsWithDifferentChance_NotAllStepsAppear()
        {
            var sut = new Transform()
                .Insert(0, "name").Chance(1.0)        // Always in output
                .Insert(0, "prefix-").Chance(0.5)     // Sometimes in output
                .Append("-suffix").Chance(0.5)        // Sometimes in output
                .Append("-doublesuffix").Chance(0.0); // Never in output

            var emptyName = new Name();
            var results = new HashSet<string>();

            for(int i = 0; i < 1000; i++)
            {
                var transformedName = sut.Apply(emptyName);
                results.Add(transformedName.ToString()); 
            }

            Assert.IsTrue(results.Count == 4);
            Assert.IsTrue(results.Contains("Name"));
            Assert.IsTrue(results.Contains("Prefix-name"));
            Assert.IsTrue(results.Contains("Prefix-name-suffix"));
            Assert.IsTrue(results.Contains("Name-suffix"));

        }

        [TestMethod]
        public void TransformSet_NoTransforms_NoTransformationsAppear()
        {
            var sut = new TransformSet();
            Assert.AreEqual(_testName.ToString(), sut.Apply(_testName).ToString());
        }

        [TestMethod]
        public void TransformSet_OneTransform_AllTransformationsAppear()
        {
            var sut = new TransformSet()
                        .Add(x => x
                            .Insert(0, "prefix")
                            .Append("suffix"));

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
        }

        [TestMethod]
        public void TransformSet_TwoTransforms_AllTransformationsAppear()
        {
            var sut = new TransformSet()
                        .Add(x => x.Insert(0, "prefix"))
                        .Add(x => x.Append("suffix"));

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
            
        }

        [TestMethod]
        public void TransformSet_TwoJoinedSets_TwoTransformationsAppear()
        {

            var set1 = new TransformSet().Add(x => x.Insert(0, "prefix"));
            var set2 = new TransformSet().Add(x => x.Append("suffix"));

            var sut = set1.Join(set2);

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));

        }

        [TestMethod]
        public void TransformSet_TwoProbabilisticTransforms_OneTransformationAppears()
        {
            var sut = new TransformSet()
                        .Add(x => x.Insert(0, "prefix"))
                        .Add(x => x.Append("suffix"))
                        .RandomlySelect(1);

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Apply(_testName).ToString().ToLower();
                Assert.IsTrue(result.StartsWith("prefix") ^ result.EndsWith("suffix"));
            }
        }


        [TestMethod]
        public void NameGeneration_DeterministicTransform_TransformationAlwaysAppears()
        {

            var sut = new NameGenerator("str", "ae")
                    .Transform(x => x.Replace(-1, "suffix"));

            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.EndsWith("suffix"));
            }

        }

        [TestMethod]
        public void NameGeneration_ProbabilisticTransform_TransformSometimesAppears()
        {
            var sut = new NameGenerator("str", "ae")
                    .SetTransform(new TransformSet()
                        .Add(x => x.Replace(-1, "suffix"))
                        .Chance(0.5));

            var appearances = 0;

            for (int i = 0; i < 100; i++)
            {
                if(sut.Next().EndsWith("suffix"))
                {
                    appearances++;
                }
            }

            Assert.IsTrue(appearances > 0 && appearances < 100);

        }

        [TestMethod]
        public void NameGeneration_ZeroChanceTransform_TransformNeverAppears()
        {
            var sut = new NameGenerator("str", "ae")
                    .Transform(x => x
                        .Replace(-1, "suffix")
                        .Chance(0));

            var transformFound = false;

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next();

                if (result.EndsWith("suffix"))
                {
                    transformFound = true;
                    break;
                };
            }

            Assert.IsFalse(transformFound);

        }

        [TestMethod]
        public void NameGeneration_MultipleDeterministicTransforms_AllTransformationsAppear()
        {

            var sut = new NameGenerator("str", "ae")
                    .SetTransform(new TransformSet()
                        .Add(x => x.Insert(0, "prefix"))
                        .Add(x => x.Append("suffix")));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next().ToString().ToLower();
                Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
            }

        }

        [TestMethod]
        public void NameGeneration_SelectOneTransformFromMany_OnlyOneAppears()
        {
            var sut = new NameGenerator("str", "ae")
                    .SetTransform(new TransformSet()
                        .Add(x => x.Insert(0, "prefix"))
                        .Add(x => x.Append("suffix"))
                        .RandomlySelect(1));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next().ToString().ToLower();
                Assert.IsTrue(result.StartsWith("prefix") ^ result.EndsWith("suffix"));
            }
        }

    }
}
