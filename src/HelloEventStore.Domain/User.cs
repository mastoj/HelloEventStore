using System;
using HelloEventStore.Domain.Events;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain
{
    public class User : AggregateBase
    {
        private string _userName;
        private string _name;

        public User()
        {
            RegisterTransition<UserCreated>(Apply);
            RegisterTransition<NameChanged>(Apply);
        }

        private void Apply(NameChanged obj)
        {
            _name = obj.NewName;
        }

        public void Apply(UserCreated @event)
        {
            Id = @event.Id;
            _userName = @event.UserName;
            _name = @event.Name;
        }

        private User(Guid id, string userName, string name) : this()
        {
            RaiseEvent(new UserCreated(id, userName, name));
        }

        public static User CreateUser(string userName, string name)
        {
            return new User(Guid.NewGuid(), userName, name);
        }

        public void ChangeName(string newName)
        {
            if (newName != _name)
            {
                RaiseEvent(new NameChanged(Id, newName));
            }
        }
    }
}