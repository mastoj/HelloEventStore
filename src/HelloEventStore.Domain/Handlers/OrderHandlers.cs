using HelloEventStore.Domain.Aggregates;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Handlers
{
    public class OrderHandlers : IHandle<PlaceOrder>, IHandle<DeliverOrder>, IHandle<CancelOrder>
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

        public IAggregate Handle(DeliverOrder command)
        {
            var order = _domainRepository.GetById<Order>(command.Id);
            order.DeliverOrder();
            return order;
        }

        public IAggregate Handle(CancelOrder command)
        {
            var order = _domainRepository.GetById<Order>(command.Id);
            order.Cancel();
            return order;
        }
    }
}