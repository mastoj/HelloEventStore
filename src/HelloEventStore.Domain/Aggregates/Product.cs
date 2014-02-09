using System;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Aggregates
{
    public class Product : AggregateBase
    {
        private int _quantity;
        private string _name;

        public Product()
        {
            RegisterTransition<ProductAddedToInventory>(Apply);
            RegisterTransition<ProductQuantityDecreased>(Apply);
            RegisterTransition<ProductQuantityIncreased>(Apply);
        }

        private void Apply(ProductQuantityChanged @event)
        {
            _quantity = _quantity + @event.QuantityChange;
        }

        private Product(Guid id, string productName, int quantity) : this()
        {
            RaiseEvent(new ProductAddedToInventory(id, productName, quantity));
        }

        private void Apply(ProductAddedToInventory @event)
        {
            Id = @event.Id;
            _quantity = @event.Quantity;
            _name = @event.ProductName;
        }

        public static IAggregate CreateProduct(string productName, int quantity)
        {
            return new Product(IdGenerator.GetId(), productName, quantity);
        }

        public void UpdateInventory(int quantityChange)
        {
            if (quantityChange > 0)
            {
                RaiseEvent(new ProductQuantityIncreased(Id, quantityChange, _quantity));
            }
            else
            {
                if(_quantity + quantityChange < 0)
                    RaiseEvent(new OutOfProduct(Id, _name));
                RaiseEvent(new ProductQuantityDecreased(Id, quantityChange, _quantity));
            }
        }
    }
}