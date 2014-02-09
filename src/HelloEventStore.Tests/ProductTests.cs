using System;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using NUnit.Framework;

namespace HelloEventStore.Tests
{
    [TestFixture]
    public class ProductTests : TestBase
    {
        [Test]
        public void AddProuctToInventory_ProductAddedToInventory()
        {
            var productName = "ball";
            int quantity = 100;
            var addProductToInventory = new AddProductToInventory(productName, quantity);
            var expectedId = Guid.NewGuid();
            IdGenerator.GuidGenerator = () => expectedId;

            When(addProductToInventory);
            Then(new ProductAddedToInventory(expectedId, productName, quantity));
        }

        [Test]
        public void UpdateProductInventory_ProductQuantityIncreased()
        {
            var initialQuantity = 100;
            var someName = "someName";
            int quantityChange = 50;
            var id = Guid.NewGuid();

            Given(id, new ProductAddedToInventory(id, someName, initialQuantity));
            When(new UpdateProductInventory(id, quantityChange));
            Then(new ProductQuantityIncreased(id, quantityChange, initialQuantity));
        }

        [Test]
        public void UpdateProductInventory_ProductQuantityDecreased()
        {
            var initialQuantity = 100;
            var someName = "someName";
            int quantityChange = -50;
            var id = Guid.NewGuid();

            Given(id, new ProductAddedToInventory(id, someName, initialQuantity));
            When(new UpdateProductInventory(id, quantityChange));
            Then(new ProductQuantityDecreased(id, quantityChange, initialQuantity));
        }

        [Test]
        public void UpdateProductInventory_OutOfProduct_WhenMoreItemsAreRemovedThenInStock()
        {
            var initialQuantity = 10;
            var someName = "someName";
            int quantityChange = -11;
            var id = Guid.NewGuid();

            Given(id, new ProductAddedToInventory(id, someName, initialQuantity));
            When(new UpdateProductInventory(id, quantityChange));
            Then(new OutOfProduct(id, someName), new ProductQuantityDecreased(id, quantityChange, initialQuantity));
        }

        [Test]
        public void UpdateProductInventory_IncreasesTheInventoryAccordingly()
        {
            var initialQuantity = 10;
            var someName = "someName";
            int quantityChange = -11;
            var id = Guid.NewGuid();

            Given(id, new ProductAddedToInventory(id, someName, initialQuantity),
                new ProductQuantityIncreased(id, 1, initialQuantity));
            When(new UpdateProductInventory(id, quantityChange));
            Then(new ProductQuantityDecreased(id, quantityChange, 11));
        }

        [Test]
        public void UpdateProductInventory_DecreasesTheInventoryAccordingly()
        {
            var initialQuantity = 11;
            var someName = "someName";
            int quantityChange = -11;
            var id = Guid.NewGuid();

            Given(id, new ProductAddedToInventory(id, someName, initialQuantity),
                new ProductQuantityDecreased(id, -1, initialQuantity));
            When(new UpdateProductInventory(id, quantityChange));
            Then(new OutOfProduct(id, someName), new ProductQuantityDecreased(id, quantityChange, 10));
        }
    }
}