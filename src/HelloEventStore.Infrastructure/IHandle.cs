namespace HelloEventStore.Infrastructure
{
    public interface IHandle<in TCommand>
    {
        IAggregate Handle(TCommand command);
    }
}