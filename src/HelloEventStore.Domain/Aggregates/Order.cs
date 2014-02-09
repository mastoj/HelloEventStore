using System;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Aggregates
{
    public class Order : AggregateBase
    {
        private int _quantity;
        private Guid _productId;

        public Order()
        {
            RegisterTransition<OrderCreated>(Apply);
        }

        private Order(Guid id, Guid userId, Guid productId, int quantity) : this()
        {
            RaiseEvent(new OrderCreated(id, userId, productId, quantity));
        }

        private void Apply(OrderCreated @event)
        {
            Id = @event.Id;
            _productId = @event.ProductId;
            _quantity = @event.Quantity;
        }

        public static Order Create(Guid userId, Guid productId, int quantity)
        {
            return new Order(IdGenerator.GetId(), userId, productId, quantity);
        }

        public void DeliverOrder()
        {
            RaiseEvent(new OrderDelivered(Id));
        }

        public void Cancel()
        {
            RaiseEvent(new OrderCancelled(Id, _productId, _quantity));
        }
    }
}