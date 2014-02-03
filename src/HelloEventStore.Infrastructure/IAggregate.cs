using System;
using System.Collections.Generic;

namespace HelloEventStore.Infrastructure
{
    public interface IAggregate
    {
        IEnumerable<object> UncommitedEvents();
        void ClearUncommitedEvents();
        int Version { get; }
        Guid Id { get; }
        void ApplyEvent(object @event);
    }
}