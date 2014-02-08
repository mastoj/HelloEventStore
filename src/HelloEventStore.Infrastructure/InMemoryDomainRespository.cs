using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI.Exceptions;

namespace HelloEventStore
{
    public class InMemoryDomainRespository : DomainRepositoryBase
    {
        public Dictionary<Guid, List<object>> _eventStore = new Dictionary<Guid, List<object>>(); 

        public override void Save<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
            if (expectedVersion <= 0)
            {
                _eventStore.Add(aggregate.Id, eventsToSave);
            }
            else
            {
                var existingEvents = _eventStore[aggregate.Id];
                var currentversion = existingEvents.Count;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException("Expected version " + expectedVersion +
                                                            " but the version is " + currentversion);
                }
                existingEvents.AddRange(eventsToSave);
            }
            aggregate.ClearUncommitedEvents();
        }

        public override TResult GetById<TResult>(Guid id)
        {
            var events = _eventStore[id];
            return BuildAggregate<TResult>(events);
        }
    }
}