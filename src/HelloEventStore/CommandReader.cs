using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Commands;
using HelloEventStore.Domain.Commands;

namespace HelloEventStore
{
    internal class CommandReader
    {
        private static Dictionary<string, Tuple<string, Func<string[], object>>> _commandParsers = new Dictionary
            <string, Tuple<string, Func<string[], object>>>()
        {
            {"ca", DescAction("po - [user name] [product id]", CreatePlaceOrderCommand)},
            {"cu", DescAction("cu [user name] [name] - create user",CreateCreateUserCommand)},
            {"cn", DescAction("cn [user name] [new name] - change user name",CreateChangeUserNameCommand)},
            {"q", DescAction("q - quit", _ => new Quit())},
            {"?", DescAction("? - printOptions", _ => new PrintOptions())},
        };

        private static object CreateChangeUserNameCommand(string[] arg)
        {
            var userName = arg[0];
            var newName = arg[1];
            var userId = UserView.Instance.GetUserId(userName);
            return new ChangeName(userId, newName);
        }

        private static object CreatePlaceOrderCommand(string[] arg)
        {
            throw new NotImplementedException();
        }

        private static Tuple<string, Func<string[], object>> DescAction(string description,
            Func<string[], object> action)
        {
            return Tuple.Create(description, action);
        }

        private static object CreateCreateUserCommand(string[] arg)
        {
            return new CreateUser(arg[0], arg[1]);
        }

        public object ReadCommand(string commandLine)
        {
            var args = commandLine.Split(' ');
            if (_commandParsers.ContainsKey(args[0]))
            {
                var descAndFunc = _commandParsers[args[0]];
                return descAndFunc.Item2(args.Skip(1).ToArray());                
            }
            return new PrintOptions();
        }

        public void PrintOptions()
        {
            Console.WriteLine("The following commands exist: ");
            _commandParsers.ToList().ForEach(y =>
            {
                Console.WriteLine(y.Value.Item1);
            });
        }
    }
}