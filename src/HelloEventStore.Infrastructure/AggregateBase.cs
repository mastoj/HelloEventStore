using System;
using System.Collections.Generic;

namespace HelloEventStore.Infrastructure
{
    public class AggregateBase : IAggregate
    {
        public int Version
        {
            get
            {
                return _version;
            }
            protected set
            {
                _version = value;
            }
        }

        public Guid Id { get; protected set; }

        private List<object> _uncommitedEvents = new List<object>();
        private Dictionary<Type, Action<object>> _routes = new Dictionary<Type, Action<object>>();
        private int _version = -1;

        public void RaiseEvent(object @event)
        {
            ApplyEvent(@event);
            _uncommitedEvents.Add(@event);
        }

        protected void RegisterTransition<T>(Action<T> transition) where T : class
        {
            _routes.Add(typeof (T), o => transition(o as T));
        }

        public void ApplyEvent(object @event)
        {
            var eventType = @event.GetType();
            if (_routes.ContainsKey(eventType))
            {
                _routes[eventType](@event);
            }
            Version++;
        }

        public IEnumerable<object> UncommitedEvents()
        {
            return _uncommitedEvents;
        }

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }
    }
}