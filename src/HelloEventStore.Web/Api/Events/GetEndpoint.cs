using System.Collections.Generic;
using HelloEventStore.Infrastructure;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;

namespace HelloEventStore.Web.Api.Events
{
    [UriTemplate("/api/events")]
    public class GetEndpoint : IGet, IOutput<IEnumerable<ObjectModel>>
    {
        public Status Get()
        {
            Output = ObjectModel.GetObjects<IEvent>();
            return Status.OK;
        }

        public IEnumerable<ObjectModel> Output { get; private set; }
    }
}