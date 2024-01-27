using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SystemMonitor.Logic.Utilities;

namespace SystemMonitor.Tests.UnitTests.Logic.Utilities
{
    [TestClass]
    public class OrderedStringListTests
    {
        [TestMethod]
        public void AddIfNotExist_StartingItem_AddsItemAtStart()
        {
            // Arrange.
            string[] items = ["a", "b", "d"];

            OrderedStringList orderedStringList = new OrderedStringList(items);

            // Act.
            orderedStringList.AddIfNotExist("c");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringList.Items.Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_IntermediateItem_AddsItemAtMiddle()
        {
            // Arrange.
            string[] items = ["b", "c", "d"];

            OrderedStringList orderedStringList = new OrderedStringList(items);

            // Act.
            orderedStringList.AddIfNotExist("a");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringList.Items.Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_EndingItem_AddsItemAtEnd()
        {
            // Arrange.
            string[] items = ["a", "b", "c"];

            OrderedStringList orderedStringList = new OrderedStringList(items);

            // Act.
            orderedStringList.AddIfNotExist("d");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringList.Items.Should().BeEquivalentTo(expectedItems);
        }

        [TestMethod]
        public void AddIfNotExist_ExistingItem_DoesNotAddItem()
        {
            // Arrange.
            string[] items = ["a", "b", "c", "d"];

            OrderedStringList orderedStringList = new OrderedStringList(items);

            // Act.
            orderedStringList.AddIfNotExist("c");

            // Assert.
            string[] expectedItems = ["a", "b", "c", "d"];

            orderedStringList.Items.Should().BeEquivalentTo(expectedItems);
        }
    }
}