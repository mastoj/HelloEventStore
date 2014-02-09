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

            var userHandlers = new UserHandlers(userView, domainRepository);
            commandDispatcher.RegisterHandler<CreateUser>(userHandlers.Handle);
            commandDispatcher.RegisterHandler<ChangeName>(userHandlers.Handle);

            var productHandlers = new ProductHandlers(domainRepository);
            commandDispatcher.RegisterHandler<AddProductToInventory>(productHandlers.Handle);
            commandDispatcher.RegisterHandler<UpdateProductInventory>(productHandlers.Handle);
            
            return commandDispatcher;
        }

        public void ExecuteCommand(object command)
        {
            _commandDispatcher.ExecuteCommand(command as ICommand);
        }
    }
}