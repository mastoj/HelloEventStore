using System;
using System.Collections.Generic;

namespace HelloEventStore.Infrastructure
{
    public interface IDomainRepository
    {
        IEnumerable<object> Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
        TResult GetById<TResult>(Guid id) where TResult : IAggregate, new();
    }
}