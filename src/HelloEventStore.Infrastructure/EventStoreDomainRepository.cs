using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using HelloEventStore.Infrastructure;
using Newtonsoft.Json;

namespace HelloEventStore
{
    public class EventStoreDomainRepository : IDomainRepository
    {
        private IEventStoreConnection _connection;

        public EventStoreDomainRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        private string AggregateToStreamName(Type type, Guid id)
        {
            return string.Format("{0} - {1}", type.FullName, id);
        }

        public void Save<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            var events = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, events);
            var eventData = events.Select(CreateEventData);
            var streamName = AggregateToStreamName(aggregate.GetType(), aggregate.Id);
            _connection.AppendToStream(streamName, expectedVersion, eventData);
        }

        public TResult GetById<TResult>(Guid id) where TResult : IAggregate, new()
        {
            var streamName = AggregateToStreamName(typeof(TResult), id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue, false);
            var aggregate = new TResult();
            foreach (var resolvedEvent in events.Events)
            {
                var metadata = DeserializeObject<Dictionary<string, string>>(resolvedEvent.OriginalEvent.Metadata);
                var eventData = DeserializeObject(resolvedEvent.OriginalEvent.Data, metadata[EventClrTypeHeader]);
                aggregate.ApplyEvent(eventData);
            }

            return aggregate;
        }

        private T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof (T).AssemblyQualifiedName));
        }

        private object DeserializeObject(byte[] data, string typeName)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
        }

        private int CalculateExpectedVersion(IAggregate aggregate, List<object> events)
        {
            var expectedVersion = aggregate.Version - events.Count - 1;
            expectedVersion = expectedVersion == -1 ? ExpectedVersion.NoStream : expectedVersion;
            return expectedVersion;
        }

        public EventData CreateEventData(object @event)
        {
            var eventHeaders = new Dictionary<string, string>()
            {
                {
                    EventClrTypeHeader, @event.GetType().AssemblyQualifiedName
                }
            };
            var eventDataHeaders = SerializeObject(eventHeaders);
            var data = SerializeObject(@event);
            var eventData = new EventData(Guid.NewGuid(), @event.GetType().Name, true, data, eventDataHeaders);
            return eventData;
        }

        private byte[] SerializeObject(object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonObj);
            return data;
        }

        public string EventClrTypeHeader = "EventClrTypeName";
    }
}