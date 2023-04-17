using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Syllabore.Tests
{
    [TestClass]
    public class NameTransformerTests
    {

        [TestMethod]
        public void NameGeneration_UseMutatorDirectly_MutationsAppear()
        {
            var name = new Name("syl", "la", "bore");
            var mutator = new TransformSet()
                            .WithTransform(x => x.AppendSyllable("test"))
                            .Join(new TransformSet()
                                .WithTransform(x => x.ReplaceSyllable(0, "test")));

            for (int i = 0; i < 20; i++)
            {
                Assert.IsTrue(name.ToString() != mutator.Apply(name).ToString());
            }

        }

        [TestMethod]
        public void NameGeneration_UsingTransform_TransformAppears()
        {

            const string stringToTest = "test!";

            var g = new NameGenerator()
                    .UsingTransform(x => x
                        .ReplaceSyllable(-1, stringToTest));

            for (int i = 0; i < 100; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.EndsWith(stringToTest));
            }

        }

        [TestMethod]
        public void NameGeneration_UsingHalfChanceTransform_TransformAppears()
        {

            const string stringToTest = "test!";

            var g = new NameGenerator()
                    .UsingTransform(0.5, new TransformSet()
                        .WithTransform(x => x.ReplaceSyllable(-1, stringToTest)));

            bool stringFound = false;
            for (int i = 0; i < 100; i++)
            {
                if(g.Next().EndsWith(stringToTest))
                {
                    stringFound = true; 
                    break;
                };
            }

            Assert.IsTrue(stringFound);

        }

        [TestMethod]
        public void NameGeneration_UsingZeroChanceTransform_TransformAppears()
        {

            const string stringToTest = "test!";

            var g = new NameGenerator()
                    .UsingTransform(0.0, x => x
                        .ReplaceSyllable(-1, stringToTest));

            bool stringFound = false;
            for (int i = 0; i < 100; i++)
            {
                if (g.Next().EndsWith(stringToTest))
                {
                    stringFound = true;
                    break;
                };
            }

            Assert.IsFalse(stringFound);

        }

        [TestMethod]
        public void NameGeneration_MultipleTransformsAllowed_TheyAllAppear()
        {

            const string stringToTest1 = "hello!";
            const string stringToTest2 = "world!";

            var g = new NameGenerator()
                    .UsingTransform(new TransformSet()
                        .WithTransform(x => x.ReplaceSyllable(-1, stringToTest1))
                        .WithTransform(x => x.AppendSyllable(stringToTest2)));

            for (int i = 0; i < 100; i++)
            {
                var name = g.Next();
                Assert.IsTrue(name.EndsWith(stringToTest1 + stringToTest2));
            }

        }

        [TestMethod]
        public void NameGeneration_MultipleTransformsButOnlyOneAllowed_OnlyOneAppears()
        {
            const string stringToTest1 = "hello!";
            const string stringToTest2 = "world!";

            var g = new NameGenerator()
                    .UsingTransform(new TransformSet()
                        .RandomlySelect(1)
                        .WithTransform(x => x.ReplaceSyllable(0, stringToTest1))
                        .WithTransform(x => x.AppendSyllable(stringToTest2)));

            for (int i = 0; i < 100; i++)
            {
                var name = g.Next().ToLower();
                Assert.IsTrue(name.StartsWith(stringToTest1) ^ name.EndsWith(stringToTest2));
            }

        }

    }
}
