using HelloEventStore.Domain.Aggregates;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Services
{
    public class ProductHandlers : IHandle<AddProductToInventory>, IHandle<UpdateProductInventory>
    {
        private readonly IDomainRepository _domainRepository;

        public ProductHandlers(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(AddProductToInventory command)
        {
            var product = Product.CreateProduct(command.ProductName, command.Quantity);
            return product;
        }

        public IAggregate Handle(UpdateProductInventory command)
        {
            var product = _domainRepository.GetById<Product>(command.Id);
            product.UpdateInventory(command.QuantityChange);
            return product;
        }
    }
}