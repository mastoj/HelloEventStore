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
        private bool _approved;
        private Guid _userId;

        public Order()
        {
            RegisterTransition<OrderCreated>(Apply);
            RegisterTransition<NeedsApproval>(Apply);
            RegisterTransition<OrderDelivered>(Apply);
        }

        private Order(Guid id, Guid userId, Guid productId, int quantity)
            : this()
        {
            if (quantity > 100)
            {
                RaiseEvent(new NeedsApproval(id, userId, productId, quantity));
            }
            else
            {
                RaiseEvent(new OrderCreated(id, userId, productId, quantity));
            }
        }

        private void Apply(OrderCreated @event)
        {
            DynamicApply(@event);
        }

        private void Apply(NeedsApproval @event)
        {
            DynamicApply(@event, false);
        }

        private void DynamicApply(dynamic dynamicEvent, bool approved = true)
        {
            Id = dynamicEvent.Id;
            _userId = dynamicEvent.UserId;
            _productId = dynamicEvent.ProductId;
            _quantity = dynamicEvent.Quantity;
            _delivered = false;
            _approved = approved;
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

        public void Approve()
        {
            RaiseEvent(new OrderCreated(Id, _userId, _productId, _quantity));
            RaiseEvent(new OrderApproved(Id));
        }
    }
}