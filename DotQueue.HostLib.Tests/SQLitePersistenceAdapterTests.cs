using System.Collections.Generic;
using DotQueue.Persistence.SQLite;
using DotQueue.Persistence.SQLite.Contracts;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class SQLitePersistenceAdapterTests
    {
        private SQLitePersistenceAdapter _persistenceAdapter;
        private ICommandQueue _queue;
        private ISqlAdapter _adapter;

        [SetUp]
        public void Setup()
        {
            _queue = Substitute.For<ICommandQueue>();
            _adapter = Substitute.For<ISqlAdapter>();
            _persistenceAdapter = new SQLitePersistenceAdapter(_adapter, _queue);
        }

        [Test]
        public void GetAll_Returns_All_Messages_From_SqlAdapter()
        {
            var expected = new List<PersistedMessage>();
            _adapter.GetAll().Returns(expected);

            var actual = _persistenceAdapter.GetAll();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
