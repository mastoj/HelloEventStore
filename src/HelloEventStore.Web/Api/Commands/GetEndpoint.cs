using System.Collections.Generic;
using HelloEventStore.Infrastructure;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Links;

namespace HelloEventStore.Web.Api.Commands
{
    [UriTemplate("/api/commands")]
    [Root(Rel = "commands", Title = "Command list", Type = "application/vnd.helloeventstore")]
    public class GetEndpoint : IGet, IOutput<IEnumerable<ObjectModel>> 
    {
        public Status Get()
        {
            Output = ObjectModel.GetObjects<ICommand>();
            return Status.OK;
        }
        public IEnumerable<ObjectModel> Output { get; private set; }
    }
}
