using System;
using System.Collections.Generic;
using EventStore.ClientAPI;
using HelloEventStore.Infrastructure;

namespace HelloEventStore
{
    public abstract class DomainRepositoryBase : IDomainRepository
    {
        public abstract void Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        public abstract TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();

        protected int CalculateExpectedVersion(IAggregate aggregate, List<object> events)
        {
            var expectedVersion = aggregate.Version - events.Count;
            return expectedVersion;
        }

        protected TResult BuildAggregate<TResult>(IEnumerable<object> events) where TResult : IAggregate, new()
        {
            var result = new TResult();
            foreach (var @event in events)
            {
                result.ApplyEvent(@event);
            }
            return result;
        }
    }
}