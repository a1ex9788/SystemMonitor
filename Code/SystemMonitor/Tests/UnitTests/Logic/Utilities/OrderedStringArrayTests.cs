using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SystemMonitor.Logic.Collections;

namespace SystemMonitor.Tests.UnitTests.Logic.Utilities
{
    [TestClass]
    public class OrderedStringArrayTests
    {
        [TestMethod]
        public void AddIfNotExist_StartingItem_AddsItemAtStart()
        {
            // Arrange.
            string[] items = ["b", "c", "d"];

            OrderedStringArray orderedStringArray = new OrderedStringArray(items);

            // Act.
            orderedStringArray.AddIfNotExist("a");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringArray.GetItems().Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_IntermediateItem_AddsItemAtMiddle()
        {
            // Arrange.
            string[] items = ["a", "b", "d"];

            OrderedStringArray orderedStringArray = new OrderedStringArray(items);

            // Act.
            orderedStringArray.AddIfNotExist("c");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringArray.GetItems().Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_EndingItem_AddsItemAtEnd()
        {
            // Arrange.
            string[] items = ["a", "b", "c"];

            OrderedStringArray orderedStringArray = new OrderedStringArray(items);

            // Act.
            orderedStringArray.AddIfNotExist("d");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringArray.GetItems().Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_ExistingItem_DoesNotAddItem()
        {
            // Arrange.
            string[] items = ["a", "b", "c", "d"];

            OrderedStringArray orderedStringArray = new OrderedStringArray(items);

            // Act.
            orderedStringArray.AddIfNotExist("c");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringArray.GetItems().Should().BeEquivalentTo(expectedItems);
        }
    }
}