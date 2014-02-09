using System;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using NUnit.Framework;

namespace HelloEventStore.Tests
{
    [TestFixture]
    public class OrderTests : AggregateTestBase
    {
        [Test]
        public void PlaceOrder_OrderPlaced_IfUserAndProductExist()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            IdGenerator.GuidGenerator = () => orderId;
            var quantity = 5;
            var placeOrder = new PlaceOrder(userId, productId, quantity);

            Given(userId, new UserCreated(userId, "john", "doe"));
            Given(productId, new ProductAddedToInventory(productId, "ball", 10));
            
            When(placeOrder);
            
            Then(new OrderCreated(orderId, userId, productId, quantity),
                new ProductQuantityDecreased(productId, -quantity, 10));
        }

        [Test]
        public void PlaceOrder_OrderPlacedAndOutOfProduct_IfUserAndProductExistAndNotEnoughInStock()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            IdGenerator.GuidGenerator = () => orderId;
            var quantity = 5;
            var placeOrder = new PlaceOrder(userId, productId, quantity);
            
            Given(userId, new UserCreated(userId, "john", "doe"));
            Given(productId, new ProductAddedToInventory(productId, "ball", 4));

            When(placeOrder);
            
            Then(new OrderCreated(orderId, userId, productId, quantity),
                new OutOfProduct(productId, "ball"),
                new ProductQuantityDecreased(productId, -quantity, 4));
        }
    }
}