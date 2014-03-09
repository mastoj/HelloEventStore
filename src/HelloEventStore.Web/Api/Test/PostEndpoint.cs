using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HelloEventStore.Domain;
using HelloEventStore.Domain.Services;
using HelloEventStore.Tests;
using HelloEventStore.Web.Api.Commands;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Web.Api.Test
{
    [UriTemplate("/api/specification")]
    public class PostEndpoint : IPost, IInput<Specification>, IOutput<SpecificationResult>
    {
        public Status Post()
        {
            IdGenerator.GuidGenerator = () => Guid.Empty;

            var preCondition = Input.PreCondition.ToDictionary(y => y.Key, y => y.Value.Select(x => x.GetObject() as IEvent));
            var command = Input.Command.GetObject() as ICommand;
            var postCondition = Input.PostCondition.Select(y => y.GetObject() as IEvent);

            var domainRepository = new InMemoryDomainRespository();
            domainRepository.AddEvents(preCondition);
            var userView = new UserView();
            var app = new HelloEventStoreApplication(userView, domainRepository);
            app.ExecuteCommand(command);
            var latestEvents = domainRepository.GetLatestEvents();
            Output = new SpecificationResult()
            {
                ExpectedEvents = postCondition,
                ResultingEvents = latestEvents
            };
            return Status.OK;
        }

        public Specification Input { set; private get; }
        public SpecificationResult Output { get; private set; }
    }

    public class Specification
    {
        public Dictionary<Guid, IEnumerable<ObjectModel>> PreCondition { get; set; }
        public ObjectModel Command { get; set; }
        public IEnumerable<ObjectModel> PostCondition { get; set; }
    }

    public class SpecificationResult
    {
        public IEnumerable<IEvent> ResultingEvents { get; set; }
        public IEnumerable<IEvent> ExpectedEvents { get; set; }
    }
}