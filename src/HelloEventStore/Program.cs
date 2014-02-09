using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using HelloEventStore.Commands;
using HelloEventStore.Domain;
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
            CreateProductView(connection);
            var userView = CreateUserView(connection);
            var domainRepository = new EventStoreDomainRepository(connection);
            var application = new HelloEventStoreApplication(userView, domainRepository, new List<Action<ICommand>>(){Logger}, new List<Action<object>>(){Logger} );

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
                    else if (command is ListUsers)
                    {
                        PrintUsers();
                    }
                    else if (command is ListProducts)
                    {
                        PrintProducts();
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

        private static void PrintUsers()
        {
            PrintAggregates(UserView.Instance.GetAll());
        }

        private static void PrintProducts()
        {
            PrintAggregates(ProductView.Instance.GetAll());
        }

        private static void PrintAggregates(Dictionary<string, Guid> aggregates)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var entry in aggregates)
            {
                Console.WriteLine("Name: {0}, Id: {1}", entry.Key, entry.Value);
            }
            Console.ForegroundColor = oldColor;
        }

        private static void CreateProductView(IEventStoreConnection connection)
        {
            var productView = ProductView.Instance;
            Position position = Position.Start;
            var allEvents = connection.ReadAllEventsForward(position, int.MaxValue, false);
            Action<ResolvedEvent> updateView = re =>
            {
                if (re.OriginalEvent.EventType == typeof(ProductAddedToInventory).Name)
                {
                    var jsonString = Encoding.UTF8.GetString(re.OriginalEvent.Data);
                    var @event = JsonConvert.DeserializeObject<ProductAddedToInventory>(jsonString);

                    productView.Insert(@event.Id, @event.ProductName);
                }
            };
            foreach (var resolvedEvent in allEvents.Events)
            {
                updateView(resolvedEvent);
            }
            connection.SubscribeToAll(false, (ess, re) => updateView(re));
        }

        private static void Logger(object obj)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            var commandType = obj.GetType();
            Console.WriteLine(commandType.Name);
            var propertyInfos = commandType.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                Console.WriteLine("{0}: {1}", propertyInfo.Name, propertyInfo.GetValue(obj, null));
            }
            Console.ForegroundColor = oldColor;
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

                    userView.Insert(@event.Id, @event.UserName);
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
