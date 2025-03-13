using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using Syllabore.Fluent;

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
            Assert.IsTrue(_testName.ToString() == sut.Apply(_testName).ToString());
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
        public void TransformSet_NoTransforms_NoTransformationsAppear()
        {
            var sut = new TransformSet();
            Assert.IsTrue(_testName.ToString() == sut.Apply(_testName).ToString());
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
                    .SetTransform(0.5, new TransformSet()
                        .Add(x => x.Replace(-1, "suffix")));

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
                    .Transform(0.0, x => x
                        .Replace(-1, "suffix"));

            var transformFound = false;

            for (int i = 0; i < 100; i++)
            {
                if (sut.Next().EndsWith("suffix"))
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
        public void NameGeneration_MultipleProbabilisticTransforms_OnlyOneAppears()
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
