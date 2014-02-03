using System;

namespace HelloEventStore.Infrastructure
{
    public interface IDomainRepository
    {
        void Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();
    }
}