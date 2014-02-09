using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore
{
    public class HelloEventStoreApplication
    {
        private readonly IEnumerable<Action<ICommand>> _preExecutionPipe;
        private CommandDispatcher _commandDispatcher;

        public HelloEventStoreApplication(IUserView userView, IDomainRepository domainRepository, params Action<ICommand>[] preExecutionPipe)
        {
            _preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            _commandDispatcher = CreateCommandDispatcher(domainRepository, userView);
        }

        private CommandDispatcher CreateCommandDispatcher(IDomainRepository domainRepository, IUserView userView)
        {
            var commandDispatcher = new CommandDispatcher(domainRepository, _preExecutionPipe);
            var userService = new UserService(userView, domainRepository);
            commandDispatcher.RegisterHandler<CreateUser>(userService.Handle);
            commandDispatcher.RegisterHandler<ChangeName>(userService.Handle);
            return commandDispatcher;
        }

        public void ExecuteCommand(object command)
        {
            _commandDispatcher.ExecuteCommand(command as ICommand);
        }
    }
}