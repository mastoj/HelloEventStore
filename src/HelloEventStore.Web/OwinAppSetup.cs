using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Simple.Web;

namespace HelloEventStore.Web
{
	using UseAction = Action<Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task>>;

    public class OwinAppSetup
    {
        public static Type[] EnforceReferencesFor =
                {
                    typeof (Simple.Web.JsonNet.JsonMediaTypeHandler),
                    typeof(Domain.Commands.CreateUser),
                    typeof(Domain.Events.OutOfProduct)
                };

        public static void Setup(UseAction use)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            var staticBuilder = Simple.Owin.Static.Statics.AddFolder("/app").AddFolder("/content").AddFileAlias("/Views/Index.html", "/");
            use(staticBuilder.Build());
            use(Application.Run);
        }
    }
}