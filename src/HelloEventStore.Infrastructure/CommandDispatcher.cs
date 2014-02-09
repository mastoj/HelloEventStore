using System;
using System.Collections.Generic;
using System.Linq;

namespace HelloEventStore.Infrastructure
{
    public class CommandDispatcher
    {
        private Dictionary<Type, Func<object, IAggregate>> _routes;
        private IDomainRepository _domainRepository;
        private readonly IEnumerable<Action<ICommand>> _preExecutionPipe;

        public CommandDispatcher(IDomainRepository domainRepository, IEnumerable<Action<ICommand>> preExecutionPipe)
        {
            _domainRepository = domainRepository;
            _preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            _routes = new Dictionary<Type, Func<object, IAggregate>>();
        }

        public void RegisterHandler<TCommand>(Func<TCommand, IAggregate> handle) where TCommand : class
        {
            _routes.Add(typeof(TCommand), o => handle(o as TCommand));
        }

        public void ExecuteCommand(ICommand command)
        {
            var commandType = command.GetType();

            RunPreExecutionPipe(command);
            if (!_routes.ContainsKey(commandType))
            {
                throw new ApplicationException("Missing handler for " + commandType.Name);
            }
            var aggregate = _routes[commandType](command);
            _domainRepository.Save(aggregate);
        }

        private void RunPreExecutionPipe(ICommand command)
        {
            foreach (var action in _preExecutionPipe)
            {
                action(command);
            }
        }
    }
}