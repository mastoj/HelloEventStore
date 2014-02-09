using System;
using System.Collections.Generic;
using System.Linq;
using HelloEventStore.Domain;
using HelloEventStore.Domain.Services;
using HelloEventStore.Infrastructure;
using NUnit.Framework;

namespace HelloEventStore.Tests
{
    public abstract class AggregateTestBase
    {
        private InMemoryDomainRespository _domainRepository;
        private HelloEventStoreApplication application_application;
        private IUserView _userView;
        private Dictionary<Guid, List<object>> _preConditions = new Dictionary<Guid, List<object>>();

        protected IUserView UserView
        {
            get
            {
                _userView = _userView ?? new UserView();
                return _userView;
            }
            set
            {
                _userView = value;
            }
        }

        private HelloEventStoreApplication BuildApplication()
        {
            _domainRepository = new InMemoryDomainRespository();
            _domainRepository.AddEvents(_preConditions);
            return new HelloEventStoreApplication(UserView, _domainRepository);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            IdGenerator.GuidGenerator = null;
            _preConditions = new Dictionary<Guid, List<object>>();
        }

        protected void When(ICommand command)
        {
            var application = BuildApplication();
            application.ExecuteCommand(command);
        }

        protected void Then(params object[] expectedEvents)
        {
            var latestEvents = _domainRepository.GetLatestEvents().ToList();
            var expectedEventsList = expectedEvents.ToList();
            Assert.AreEqual(expectedEventsList.Count, latestEvents.Count);

            for (int i = 0; i < latestEvents.Count; i++)
            {
                Assert.AreEqual(expectedEvents[i], latestEvents[i]);
            }
        }

        protected void WhenThrows<TException>(ICommand command) where TException : Exception
        {
            Assert.Throws<TException>(() => When(command));
        }

        protected void Given(Guid id, params object[] existingEvents)
        {
            _preConditions.Add(id, new List<object>());
            _preConditions[id].AddRange(existingEvents);
        }
    }
}