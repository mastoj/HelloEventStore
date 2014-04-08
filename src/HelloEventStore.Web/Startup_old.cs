//using System;
//using Fix;
//using Newtonsoft.Json;
//using Owin;
//using Simple.Web;
//using Simple.Web.OwinSupport;

//namespace HelloEventStore.Web
//{
//    public class Startup_old
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
//            {
//                TypeNameHandling = TypeNameHandling.Objects
//            };
//            //var staticBuilder = Simple.Owin.Static.Statics.AddFolder("/app").AddFolder("/content").AddFileAlias("/Views/Index.html", "/");
//            //var appApp = new Fixer()
//            //    .Use(staticBuilder.Build())
//            //    .Use(Application.Run) // Simple.Web
//            //    .Build();

//            app.UseSimpleWeb();
//            //app.Use((context, next) =>
//            //{
//            //    return Application.Run(context.Environment, next);
//            //});
//            //app.Run(context =>
//            //{
//            //    return Application.Run(context.Environment, context.)
//            //});

//            //app.Use(appApp);
//            //app.Use(staticBuilder.Build())
//            //    .Use(appApp);
//            //app.Run(context =>
//            //{
//            //    string t = DateTime.Now.Millisecond.ToString();
//            //    return context.Response.WriteAsync(t + " Production OWIN App");
//            //});
//        }
//    }
//}