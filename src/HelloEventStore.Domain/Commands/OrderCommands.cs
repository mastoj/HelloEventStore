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
}
