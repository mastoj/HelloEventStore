using System;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Aggregates
{
    public class Order : AggregateBase
    {
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
        }

        public static Order Create(Guid userId, Guid productId, int quantity)
        {
            return new Order(IdGenerator.GetId(), userId, productId, quantity);
        }
    }
}