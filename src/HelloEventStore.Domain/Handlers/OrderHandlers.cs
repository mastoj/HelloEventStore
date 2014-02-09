using HelloEventStore.Domain.Aggregates;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Handlers
{
    public class OrderHandlers : IHandle<PlaceOrder>
    {
        private readonly IDomainRepository _domainRepository;

        public OrderHandlers(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(PlaceOrder command)
        {
            var user = _domainRepository.GetById<User>(command.UserId);
            var product = _domainRepository.GetById<Product>(command.ProductId);

            return Order.Create(command.UserId, command.ProductId, command.Quantity);
        }
    }
}