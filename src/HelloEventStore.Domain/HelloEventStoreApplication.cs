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
        private CommandDispatcher _commandDispatcher;

        public HelloEventStoreApplication(IUserView userView, IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe = null, IEnumerable<Action<object>> postExecutionPipe = null)
        {
            preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            postExecutionPipe = CreatePostExecutionPipe(postExecutionPipe);
            _commandDispatcher = CreateCommandDispatcher(userView, domainRepository, preExecutionPipe, postExecutionPipe);
        }

        public void ExecuteCommand(object command)
        {
            _commandDispatcher.ExecuteCommand(command as ICommand);
        }

        private CommandDispatcher CreateCommandDispatcher(IUserView userView, IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe, IEnumerable<Action<object>> postExecutionPipe)
        {
            var commandDispatcher = new CommandDispatcher(domainRepository, preExecutionPipe, postExecutionPipe);

            var userHandlers = new UserHandlers(userView, domainRepository);
            commandDispatcher.RegisterHandler<CreateUser>(userHandlers.Handle);
            commandDispatcher.RegisterHandler<ChangeName>(userHandlers.Handle);

            var productHandlers = new ProductHandlers(domainRepository);
            commandDispatcher.RegisterHandler<AddProductToInventory>(productHandlers.Handle);
            commandDispatcher.RegisterHandler<UpdateProductInventory>(productHandlers.Handle);

            var orderHandlers = new OrderHandlers(domainRepository);
            commandDispatcher.RegisterHandler<PlaceOrder>(orderHandlers.Handle);
            commandDispatcher.RegisterHandler<DeliverOrder>(orderHandlers.Handle);
            commandDispatcher.RegisterHandler<CancelOrder>(orderHandlers.Handle);
            commandDispatcher.RegisterHandler<ApproveOrder>(orderHandlers.Handle);

            return commandDispatcher;
        }

        private IEnumerable<Action<object>> CreatePostExecutionPipe(IEnumerable<Action<object>> postExecutionPipe)
        {
            yield return OrderPlacedHandler;
            yield return OrderCancelledHandler;
            if (postExecutionPipe != null)
            {
                foreach (var action in postExecutionPipe)
                {
                    yield return action;
                }
            }
        }

        private void OrderCancelledHandler(object obj)
        {
            var type = obj.GetType();
            if (type == typeof(OrderCancelled))
            {
                var @event = obj as OrderCancelled;
                ExecuteCommand(new UpdateProductInventory(@event.ProductId, @event.Quantity));
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
    }
}