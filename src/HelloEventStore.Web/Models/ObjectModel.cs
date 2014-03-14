using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HelloEventStore.Web.Models
{
    public class ObjectModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string QualifiedName { get; set; }
        public IEnumerable<Argument> Arguments { get; set; }

        public object GetObject()
        {
            var type = System.Type.GetType(QualifiedName);
            var result = Activator.CreateInstance(type, GetArguments(Arguments));
            return result;
        }

        private object[] GetArguments(IEnumerable<Argument> arguments)
        {
            var args = arguments.Select(y =>
            {
                var argType = System.Type.GetType(y.Type);
                var typeConverter = TypeDescriptor.GetConverter(argType);
                var value = typeConverter.ConvertFrom(y.Value);
                return value;
            }).ToArray();
            return args;
        }


        public static IEnumerable<ObjectModel> GetObjects<T>()
        {
            var iCommandType = typeof(T);
            var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(y => y.GetTypes()
                    .Where(t => t.GetInterface(iCommandType.FullName) != null));
            var commandModels = commandTypes.Select(y => new ObjectModel()
            {
                Type = y.FullName,
                Name = y.Name,
                QualifiedName = y.AssemblyQualifiedName,
                Arguments = CreateArguments(y)
            });
            return commandModels.OrderBy(y => y.Name);
        }

        private static IEnumerable<Argument> CreateArguments(Type type)
        {
            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters();
            var i = 0;
            foreach (var parameter in parameters)
            {
                yield return new Argument()
                {
                    Name = parameter.Name,
                    Type = parameter.ParameterType.FullName,
                    Order = i
                };
                i++;
            }
        }
    }
}