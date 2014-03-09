using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Domain.Commands;
using HelloEventStore.Infrastructure;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;

namespace HelloEventStore.Web.Api.Commands
{
    [UriTemplate("/api/commands")]
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
