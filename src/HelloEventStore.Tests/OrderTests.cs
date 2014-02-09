using System;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Exceptions;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;
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

        public void DeliverOrder_MarksProductAsDelivered()
        {
            Guid productId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();
            int orderQuantity = 5;

            Given(orderId, new OrderCreated(orderId, Guid.Empty, productId, orderQuantity));
            When(new DeliverOrder(orderId));
            Then(new OrderDelivered(orderId));
        }

        [Test]
        public void CancelOrder_OrderCancelledAndInventoryUpdated()
        {
            Guid productId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();
            int initialQuantity = 10;
            int orderQuantity = 5;

            Given(productId, new ProductAddedToInventory(productId, "ball", initialQuantity));
            Given(orderId, new OrderCreated(orderId, Guid.Empty, productId, orderQuantity));
            When(new CancelOrder(orderId));
            Then(new OrderCancelled(orderId), new ProductQuantityIncreased(productId, orderQuantity, 10));
        }

        [Test]
        public void CancelOrder_ThrowsExceptionIfAlreadyDelivered()
        {
            Guid productId = Guid.NewGuid();
            Guid orderId = Guid.NewGuid();
            int orderQuantity = 5;

            Given(orderId, new OrderCreated(orderId, Guid.Empty, productId, orderQuantity), new OrderDelivered(orderId));
            WhenThrows<OrderStateException>(new CancelOrder(orderId));
        }
    }
}