using System;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Exceptions;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Aggregates
{
    public class Order : AggregateBase
    {
        private int _quantity;
        private Guid _productId;
        private bool _delivered;

        public Order()
        {
            RegisterTransition<OrderCreated>(Apply);
            RegisterTransition<OrderDelivered>(Apply);
        }

        private Order(Guid id, Guid userId, Guid productId, int quantity)
            : this()
        {
            RaiseEvent(new OrderCreated(id, userId, productId, quantity));
            if (quantity > 100)
            {
                RaiseEvent(new NeedsApproval(id));                
            }
        }

        private void Apply(OrderCreated @event)
        {
            Id = @event.Id;
            _productId = @event.ProductId;
            _quantity = @event.Quantity;
            _delivered = false;
        }

        private void Apply(OrderDelivered obj)
        {
            _delivered = true;
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
            if (_delivered)
            {
                throw new OrderStateException("Order " + Id + " is already delivered and can't be cancelled");
            }
            RaiseEvent(new OrderCancelled(Id, _productId, _quantity));
        }
    }
}