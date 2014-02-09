using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI.Exceptions;
using HelloEventStore.Infrastructure;
using HelloEventStore.Infrastructure.Exceptions;

namespace HelloEventStore.Tests
{
    public class InMemoryDomainRespository : DomainRepositoryBase
    {
        public Dictionary<Guid, List<object>> _eventStore = new Dictionary<Guid, List<object>>();
        private List<object> _latestEvents = new List<object>();

        public override IEnumerable<object> Save<TAggregate>(TAggregate aggregate)
        {
            var eventsToSave = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, eventsToSave);
            if (expectedVersion < 0)
            {
                _eventStore.Add(aggregate.Id, eventsToSave);
            }
            else
            {
                var existingEvents = _eventStore[aggregate.Id];
                var currentversion = existingEvents.Count - 1;
                if (currentversion != expectedVersion)
                {
                    throw new WrongExpectedVersionException("Expected version " + expectedVersion +
                                                            " but the version is " + currentversion);
                }
                existingEvents.AddRange(eventsToSave);
            }
            _latestEvents.AddRange(eventsToSave);
            aggregate.ClearUncommitedEvents();
            return eventsToSave;
        }

        public IEnumerable<object> GetLatestEvents()
        {
            return _latestEvents;
        }

        public override TResult GetById<TResult>(Guid id)
        {
            if (_eventStore.ContainsKey(id))
            {
                var events = _eventStore[id];
                return BuildAggregate<TResult>(events);                
            }
            throw new AggregateNotFoundException("Could not found aggregate of type " + typeof (TResult) + " and id " + id);
        }

        public void AddEvents(Dictionary<Guid, List<object>> eventsForAggregates)
        {
            foreach (var eventsForAggregate in eventsForAggregates)
            {
                _eventStore.Add(eventsForAggregate.Key, eventsForAggregate.Value);
            }
        }
    }
}