using System;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Commands
{
    public class PlaceOrder : ICommand
    {
        public Guid UserId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        public PlaceOrder(Guid userId, Guid productId, int quantity)
        {
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }
    }

    public class DeliverOrder : ICommand
    {
        public Guid Id { get; private set; }

        public DeliverOrder(Guid id)
        {
            Id = id;
        }
    }

    public class CancelOrder : ICommand
    {
        public Guid Id { get; private set; }

        public CancelOrder(Guid id)
        {
            Id = id;
        }
    }
}
