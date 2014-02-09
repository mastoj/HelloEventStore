using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using HelloEventStore.Commands;
using HelloEventStore.Domain.Events;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;
using Newtonsoft.Json;

namespace HelloEventStore
{
    class Program
    {
        public static Dictionary<int, Guid> _accountDictionary = new Dictionary<int, Guid>(); 

        static void Main(string[] args)
        {
            var commandReader = new CommandReader();

            var connection = CreateConnection();
            var userView = CreateUserView(connection);
            var domainRepository = new EventStoreDomainRepository(connection);
            var application = new HelloEventStoreApplication(userView, domainRepository, Logger);

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

        private static void Logger(ICommand command)
        {
            var commandType = command.GetType();
            Console.WriteLine(commandType.Name);
            var propertyInfos = commandType.GetProperties();

            foreach (var propertyInfo in propertyInfos)
            {
                Console.WriteLine("{0}: {1}", propertyInfo.Name, propertyInfo.GetValue(command, null));
            }
        }

        private static IUserView CreateUserView(IEventStoreConnection connection)
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
    }
}
