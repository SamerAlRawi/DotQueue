using System;
using System.Net;
using System.Threading;
using DotQueue.Client;
using DotQueue.HostLib;
using NUnit.Framework;

namespace DotQueue.Persistence.SQLite.Tests
{
    [TestFixture, Category("no_ci")]
    public class SQLitePersistenceTests
    {
        [Test]
        public void Messages_Are_Persisted()
        {
            int port = 8083;
            var host = new QueueHost(port, persistenceAdapter: new SQLitePersistenceAdapter());

            //start host with SQLite persistence adapter
            new Thread(() => host.Start()).Start();
            
            //Wait for listener to be ready
            Thread.Sleep(1000);
            
            //send messages
            var client = new MessageQueue<DummyMessage>(new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = port
            });
            
            var msg1 = CreateRandomMessage();
            var msg2 = CreateRandomMessage();
            client.Add(msg1);
            client.Add(msg2);

            //stop the host
            host.Stop();

            //start the queue again
            new Thread(() => host.Start()).Start();

            //Wait for listener to be ready
            Thread.Sleep(1000);

            //pull messages and assert results are matching
            var actual1 = client.Pull();
            var actual2 = client.Pull();
            Assert.That(actual1, Is.EqualTo(msg1));
            Assert.That(actual2, Is.EqualTo(msg2));
        }

        private DummyMessage CreateRandomMessage()
        {
            return new DummyMessage
            {
                AccountNumber = new Random(1000).Next(10000),
                Name = Guid.NewGuid().ToString(),
                ZipCode = new Random(12345).Next(54321)
            };
        }
    }
}