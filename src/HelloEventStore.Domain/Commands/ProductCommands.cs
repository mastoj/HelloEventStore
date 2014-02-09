using System;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Commands
{
    public class AddProductToInventory : ICommand
    {
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }

        public AddProductToInventory(string productName, int quantity)
        {
            ProductName = productName;
            Quantity = quantity;
        }
    }

    public class UpdateProductInventory : ICommand
    {
        public Guid Id { get; private set; }
        public int QuantityChange { get; private set; }

        public UpdateProductInventory(Guid id, int quantityChange)
        {
            Id = id;
            QuantityChange = quantityChange;
        }
    }
}
