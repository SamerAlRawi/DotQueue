using DotQueue.Persistence.RavenDB;
using NSubstitute;
using NUnit.Framework;
using Raven.Client;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class RavenDbPersistenceAdapterTests
    {
        private RavenDbPersistenceAdapter _repository;
        private IDocumentStore _documentStore;
        private IDocumentSession _defaultSession;

        [SetUp]
        public void Setup()
        {
            _defaultSession = Substitute.For<IDocumentSession>();
            _documentStore = Substitute.For<IDocumentStore>();
            _documentStore.OpenSession().Returns(_defaultSession);
            _repository = new RavenDbPersistenceAdapter(_documentStore);
        }

        [Test]
        public void Initialize_Store()
        {
            _documentStore.Received().Initialize();
        }

        [Test]
        public void Add_Adds_Customer_To_Store()
        {
            var Message = new Message();
            _repository.Add(Message);

            Received.InOrder(() =>
            {
                _defaultSession.Store(Message);
                _defaultSession.SaveChanges();
                _defaultSession.Dispose();
            });
        }

        [Test]
        public void Delete_Retrieves_Customer_And_Delete_From_Store()
        {
            string id = "1001";
            var Message = new Message { Id = id };
            var instance = new Message();

            _defaultSession.Load<Message>(id).Returns(instance);

            _repository.Delete(Message);

            Received.InOrder(() =>
            {
                _defaultSession.Delete(instance);
                _defaultSession.SaveChanges();
                _defaultSession.Dispose();
            });
        }
    }
}
