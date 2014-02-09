using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Handlers;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain
{
    public class HelloEventStoreApplication
    {
        private readonly IEnumerable<Action<ICommand>> _preExecutionPipe;
        private readonly IEnumerable<Action<object>> _postExecutionPipe; 
        private CommandDispatcher _commandDispatcher;

        public HelloEventStoreApplication(IUserView userView, IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe = null, IEnumerable<Action<object>> postExecutionPipe = null)
        {
            _preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            _postExecutionPipe = CreatePostExecutionPipe(postExecutionPipe);
            _commandDispatcher = CreateCommandDispatcher(domainRepository, userView);
        }

        private IEnumerable<Action<object>> CreatePostExecutionPipe(IEnumerable<Action<object>> postExecutionPipe)
        {
            yield return OrderPlacedHandler;
            if (postExecutionPipe != null)
            {
                foreach (var action in postExecutionPipe)
                {
                    yield return action;
                }
            }
        }

        private void OrderPlacedHandler(object obj)
        {
            var type = obj.GetType();
            if (type == typeof (OrderCreated))
            {
                var @event = obj as OrderCreated;
                ExecuteCommand(new UpdateProductInventory(@event.ProductId, -@event.Quantity));
            }
        }

        private CommandDispatcher CreateCommandDispatcher(IDomainRepository domainRepository, IUserView userView)
        {
            var commandDispatcher = new CommandDispatcher(domainRepository, _preExecutionPipe, _postExecutionPipe);

            var userHandlers = new UserHandlers(userView, domainRepository);
            commandDispatcher.RegisterHandler<CreateUser>(userHandlers.Handle);
            commandDispatcher.RegisterHandler<ChangeName>(userHandlers.Handle);

            var productHandlers = new ProductHandlers(domainRepository);
            commandDispatcher.RegisterHandler<AddProductToInventory>(productHandlers.Handle);
            commandDispatcher.RegisterHandler<UpdateProductInventory>(productHandlers.Handle);

            var orderHandlers = new OrderHandlers(domainRepository);
            commandDispatcher.RegisterHandler<PlaceOrder>(orderHandlers.Handle);
            return commandDispatcher;
        }

        public void ExecuteCommand(object command)
        {
            _commandDispatcher.ExecuteCommand(command as ICommand);
        }
    }
}