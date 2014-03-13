using System;
using Fix;
using Newtonsoft.Json;
using Nowin;
using Simple.Web;

namespace HelloEventStore.Web
{
    class Program
    {
        public static Type[] EnforceReferencesFor =
                {
                    typeof (Simple.Web.Razor.RazorHtmlMediaTypeHandler),
                    typeof (Simple.Web.JsonNet.JsonMediaTypeHandler),
                    typeof(Domain.Commands.CreateUser),
                    typeof(Domain.Events.OutOfProduct)
                };

        static void Main(string[] args)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            var staticBuilder = Simple.Owin.Static.Statics.AddFolder("/app");


            // Build the OWIN app
            var app = new Fixer()
                .Use(staticBuilder.Build())
                .Use(Application.Run) // Simple.Web
                .Build();

            // Set up the Nowin server
            var builder = ServerBuilder.New()
                .SetPort(1337)
                .SetOwinApp(app);

            // Run
            using (builder.Start())
            {
                Console.WriteLine("Listening on port 1337. Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
