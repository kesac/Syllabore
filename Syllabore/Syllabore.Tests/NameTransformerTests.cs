using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

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
                .InsertSyllable(0, "prefix")
                .AppendSyllable("suffix");

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
                        .WithTransform(x => x
                            .InsertSyllable(0, "prefix")
                            .AppendSyllable("suffix"));

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
        }

        [TestMethod]
        public void TransformSet_TwoTransforms_AllTransformationsAppear()
        {
            var sut = new TransformSet()
                        .WithTransform(x => x.InsertSyllable(0, "prefix"))
                        .WithTransform(x => x.AppendSyllable("suffix"));

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
            
        }

        [TestMethod]
        public void TransformSet_TwoJoinedSets_TwoTransformationsAppear()
        {

            var set1 = new TransformSet().WithTransform(x => x.InsertSyllable(0, "prefix"));
            var set2 = new TransformSet().WithTransform(x => x.AppendSyllable("suffix"));

            var sut = set1.Join(set2);

            var result = sut.Apply(_testName).ToString().ToLower();
            Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));

        }

        [TestMethod]
        public void TransformSet_TwoProbabilisticTransforms_OneTransformationAppears()
        {
            var sut = new TransformSet()
                        .WithTransform(x => x.InsertSyllable(0, "prefix"))
                        .WithTransform(x => x.AppendSyllable("suffix"))
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

            var sut = new NameGenerator()
                    .UsingTransform(x => x
                        .ReplaceSyllable(-1, "suffix"));

            for (int i = 0; i < 100; i++)
            {
                var name = sut.Next();
                Assert.IsTrue(name.EndsWith("suffix"));
            }

        }

        [TestMethod]
        public void NameGeneration_ProbabilisticTransform_TransformSometimesAppears()
        {
            var sut = new NameGenerator()
                    .UsingTransform(0.5, new TransformSet()
                        .WithTransform(x => x.ReplaceSyllable(-1, "suffix")));

            var appearances = 0;

            for (int i = 0; i < 100; i++)
            {
                if(sut.Next().EndsWith("suffix"))
                {
                    appearances++;
                };
            }

            Assert.IsTrue(appearances > 0 && appearances < 100);

        }

        [TestMethod]
        public void NameGeneration_ZeroChanceTransform_TransformNeverAppears()
        {
            var sut = new NameGenerator()
                    .UsingTransform(0.0, x => x
                        .ReplaceSyllable(-1, "suffix"));

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

            var sut = new NameGenerator()
                    .UsingTransform(new TransformSet()
                        .WithTransform(x => x.InsertSyllable(0, "prefix"))
                        .WithTransform(x => x.AppendSyllable("suffix")));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next().ToString().ToLower();
                Assert.IsTrue(result.StartsWith("prefix") && result.EndsWith("suffix"));
            }

        }

        [TestMethod]
        public void NameGeneration_MultipleProbabilisticTransforms_OnlyOneAppears()
        {
            var sut = new NameGenerator()
                    .UsingTransform(new TransformSet()
                        .WithTransform(x => x.InsertSyllable(0, "prefix"))
                        .WithTransform(x => x.AppendSyllable("suffix"))
                        .RandomlySelect(1));

            for (int i = 0; i < 100; i++)
            {
                var result = sut.Next().ToString().ToLower();
                Assert.IsTrue(result.StartsWith("prefix") ^ result.EndsWith("suffix"));
            }
        }

    }
}
