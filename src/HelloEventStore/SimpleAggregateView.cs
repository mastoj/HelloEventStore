using System;
using System.Collections.Generic;

namespace HelloEventStore
{
    public abstract class SimpleAggregateView
    {
        private Dictionary<string, Guid> _aggregates = new Dictionary<string, Guid>();

        public bool Exist(string name)
        {
            return _aggregates.ContainsKey(name);
        }

        public void Insert(Guid id, string name)
        {
            _aggregates.Add(name, id);
        }

        public Guid GetId(string name)
        {
            return _aggregates[name];
        }

        public Dictionary<string, Guid> GetAll()
        {
            return _aggregates;
        } 
    }
}