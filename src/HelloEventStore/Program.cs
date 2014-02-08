using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using HelloEventStore.Commands;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;
using Newtonsoft.Json;

namespace HelloEventStore
{
    public class HelloEventStoreApplication
    {
        private CommandDispatcher _commandDispatcher;

        public void Configure()
        {
            var connection = CreateConnection();
            var userView = CreateUserView(connection);
            var domainRepository = new EventStoreDomainRepository(connection);
            _commandDispatcher = CreateCommandDispatcher(domainRepository, userView); 
        }

        private IUserView CreateUserView(IEventStoreConnection connection)
        {
            var userView = UserView.Instance;
            Position position = Position.Start;
            var allEvents = connection.ReadAllEventsForward(position, int.MaxValue, false);
            Action<ResolvedEvent> updateView = re =>
            {
                if (re.OriginalEvent.EventType == typeof(UserCreated).Name)
                {
                    var jsonString = Encoding.UTF8.GetString(re.OriginalEvent.Data);
                    var @event = JsonConvert.DeserializeObject<UserCreated>(jsonString);

                    userView.InsertUser(@event.Id, @event.UserName);
                }
            };
            foreach (var resolvedEvent in allEvents.Events)
            {
                updateView(resolvedEvent);
            }
            connection.SubscribeToAll(false, (ess, re) => updateView(re));
            return userView;
        }


        private CommandDispatcher CreateCommandDispatcher(EventStoreDomainRepository domainRepository, IUserView userView)
        {
            var commandDispatcher = new CommandDispatcher(domainRepository);
            var userService = new UserService(userView, domainRepository);
            commandDispatcher.RegisterHandler<CreateUser>(userService.Handle);
            commandDispatcher.RegisterHandler<ChangeName>(userService.Handle);
            return commandDispatcher;
        }

        private static IEventStoreConnection CreateConnection()
        {
            ConnectionSettings settings =
                ConnectionSettings.Create()
                    .UseConsoleLogger()
                    .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));
            var endPoint = new IPEndPoint(IPAddress.Loopback, 1113);
            var connection = EventStoreConnection.Create(settings, endPoint, null);
            connection.Connect();
            return connection;
        }

        public void ExecuteCommand(object command)
        {
            _commandDispatcher.ExecuteCommand(command as ICommand);
        }
    }

    class Program
    {
        public static Dictionary<int, Guid> _accountDictionary = new Dictionary<int, Guid>(); 

        static void Main(string[] args)
        {
            var commandReader = new CommandReader();

            var application = new HelloEventStoreApplication();
            application.Configure();

            commandReader.PrintOptions();

            var done = false;
            while (!done)
            {
                try
                {
                    var commandLine = Console.ReadLine();
                    var command = commandReader.ReadCommand(commandLine);
                    if (command is Quit)
                    {
                        done = true;
                    }
                    else if (command is PrintOptions)
                    {
                        commandReader.PrintOptions();
                    }
                    else
                    {
                        application.ExecuteCommand(command);
                    }
                }
                catch (Exception ex)
                {
                    var currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong: " + ex.Message);
                    Console.ForegroundColor = currentColor;
                }
            }
        }
    }
}
