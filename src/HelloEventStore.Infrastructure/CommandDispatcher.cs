using System;
using System.Collections.Generic;

namespace HelloEventStore.Infrastructure
{
    public class CommandDispatcher
    {
        private Dictionary<Type, Func<object, IAggregate>> _routes;
        private IDomainRepository _domainRepository;

        public CommandDispatcher(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
            _routes = new Dictionary<Type, Func<object, IAggregate>>();
        }

        public void RegisterHandler<TCommand>(Func<TCommand, IAggregate> handle) where TCommand : class
        {
            _routes.Add(typeof(TCommand), o => handle(o as TCommand));
        }

        public void ExecuteCommand(ICommand command)
        {
            var commandType = command.GetType();
            Console.WriteLine(commandType.Name);
            var propertyInfos = commandType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                Console.WriteLine("{0}: {1}", propertyInfo.Name, propertyInfo.GetValue(command, null));
            }
            if (!_routes.ContainsKey(commandType))
            {
                throw new ApplicationException("Missing handler for " + commandType.Name);
            }
            var aggregate = _routes[commandType](command);
            _domainRepository.Save(aggregate);
        }
    }
}