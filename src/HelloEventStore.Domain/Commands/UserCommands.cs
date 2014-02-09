using System;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Commands
{
    public class ChangeName : ICommand
    {
        public Guid Id { get; private set; }
        public string NewName { get; private set; }

        public ChangeName(Guid userId, string newName)
        {
            Id = userId;
            NewName = newName;
        }
    }

    public class CreateUser : ICommand
    {
        public string UserName { get; private set; }
        public string Name { get; private set; }

        public CreateUser(string userName, string name)
        {
            UserName = userName;
            Name = name;
        }
    }
}