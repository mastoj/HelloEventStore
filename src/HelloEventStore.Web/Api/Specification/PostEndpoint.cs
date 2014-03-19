using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Domain;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;
using HelloEventStore.Web.Models;
using Simple.Web;
using Simple.Web.Behaviors;
using Simple.Web.Links;

namespace HelloEventStore.Web.Api.Specification
{
    [UriTemplate("/api/specification")]
    [Root(Rel = "runspec", Title = "Execute specification")]
    public class PostEndpoint : IPost, IInput<Specification>, IOutput<SpecificationResult>
    {
        public Status Post()
        {
            IdGenerator.GuidGenerator = () => Guid.Empty;

            var preCondition = Input.PreCondition.ToDictionary(y => y.Key, y => y.Value.Select(x => x.GetObject() as IEvent));
            var command = Input.Action.GetObject() as ICommand;
            var postCondition = Input.PostCondition.Select(y => y.GetObject() as IEvent);

            var domainRepository = new InMemoryDomainRespository();
            domainRepository.AddEvents(preCondition);
            var userView = new UserView();
            var app = new HelloEventStoreApplication(userView, domainRepository);
            app.ExecuteCommand(command);
            var latestEvents = domainRepository.GetLatestEvents();
            Output = CreateResult(postCondition, latestEvents);
            return Status.OK;
        }

        private SpecificationResult CreateResult(IEnumerable<IEvent> postCondition, IEnumerable<IEvent> latestEvents)
        {
            var postConditionList = postCondition.ToList();
            var latestEventsList = latestEvents.ToList();
            var missingEvents = postConditionList.Where(y =>
            {
                if (latestEventsList.Contains(y))
                {
                    latestEventsList.RemoveAt(latestEventsList.IndexOf(latestEventsList.First(x => x.Equals(y))));
                    return false;
                }
                return true;
            }).ToList();
            var extraEvents = latestEvents.Where(y =>
            {
                if (postConditionList.Contains(y))
                {
                    postConditionList.RemoveAt(postConditionList.IndexOf(postConditionList.First(x => x.Equals(y))));
                    return false;
                }
                return true;
            }).ToList();
            return new SpecificationResult()
            {
                ExpectedEvents = postCondition,
                ResultingEvents = latestEvents,
                MissingEvents = missingEvents,
                UnexpectedEvents = extraEvents,
                Success = missingEvents.Count == 0 && extraEvents.Count == 0
            };
        }

        public Specification Input { set; private get; }
        public SpecificationResult Output { get; private set; }
    }

    public class Specification
    {
        public Dictionary<Guid, IEnumerable<ObjectModel>> PreCondition { get; set; }
        public ObjectModel Action { get; set; }
        public IEnumerable<ObjectModel> PostCondition { get; set; }
    }

    public class SpecificationResult
    {
        public IEnumerable<IEvent> ResultingEvents { get; set; }
        public IEnumerable<IEvent> ExpectedEvents { get; set; }
        public IEnumerable<IEvent> MissingEvents { get; set; }
        public IEnumerable<IEvent> UnexpectedEvents { get; set; }
        public bool Success { get; set; }
    }
}