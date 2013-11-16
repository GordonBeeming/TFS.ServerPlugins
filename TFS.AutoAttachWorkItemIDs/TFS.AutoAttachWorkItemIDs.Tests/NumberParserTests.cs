using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TFS.AutoAttachWorkItemIDs.Tests
{
    [TestClass]
    public class NumberParserTests
    {
        [TestMethod]
        public void ctor()
        {
            var parser = new NumberParser();
            Assert.IsInstanceOfType(parser, typeof(NumberParser));
        }

        [TestMethod]
        public void Parse_withNullString_ShouldReurnEmptyList()
        {
            //setup
            var parser = new NumberParser();
            string input = null;

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void Parse_withStringWithNoHashNumber_ShouldReurnEmptyList()
        {
            //setup
            var parser = new NumberParser();
            string input = "This is my comment.";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            Assert.IsTrue(actual.Count == 0);
        }

        [TestMethod]
        public void Parse_withStringWithHash2NumberNoSpace_ShouldReurnNumberInList()
        {
            //setup
            var parser = new NumberParser();
            List<int> expected = new List<int>() { 45 };
            string input = "Work on item #45";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Parse_withStringWithHash2NumberSpace_ShouldReurnNumberInList()
        {
            //setup
            var parser = new NumberParser();
            List<int> expected = new List<int>() { 45 };
            string input = "Work on item #45 and updated version number.";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Parse_withStringWithHash3NumberSpace_ShouldReurnNumberInList()
        {
            //setup
            var parser = new NumberParser();
            List<int> expected = new List<int>() { 453 };
            string input = "Work on item #453 and updated version number.";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Parse_withStringWithMultipleHashNumbers_ShouldReurnNumbersInList()
        {
            //setup
            var parser = new NumberParser();
            List<int> expected = new List<int>() { 12, 453 };
            string input = "Work on item #453 and #12.";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Parse_withStringWithJustNumbers_ShouldReurnNoNumbersInList()
        {
            //setup
            var parser = new NumberParser();
            List<int> expected = new List<int>();
            string input = "Work on item 453 and 12.";

            //execute
            List<int> actual = parser.Parse(input);

            //asert
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
