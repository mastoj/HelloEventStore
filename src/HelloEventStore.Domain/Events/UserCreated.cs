using System;

namespace HelloEventStore.Domain.Events
{
    public class UserCreated 
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Name { get; private set; }

        public UserCreated(Guid id, string userName, string name)
        {
            Id = id;
            UserName = userName;
            Name = name;
        }
    }

    public class NameChanged
    {
        public Guid Id { get; private set; }
        public string NewName { get; private set; }

        public NameChanged(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }
    }
}