﻿using System.Collections.Generic;
using HelloEventStore.Infrastructure;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Links;

namespace HelloEventStore.Web.Api.Events
{
    [UriTemplate("/api/events")]
    //[Canonical(typeof(IEnumerable<ObjectModel>), Title = "Event list", Type = "application/vnd.helloeventstore")]
    [Root(Rel = "events", Title = "Event list", Type = "application/vnd.helloeventstore")]
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