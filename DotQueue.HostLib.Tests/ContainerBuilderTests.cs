using DotQueue.HostLib.IOC;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class ContainerBuilderTests
    {
        [Test]
        public void Resolves_ISubscriptionService_From_Singleton()
        {
            var container = ContainerBuilder.GetContainer();

            var instance1 = container.GetService(typeof(ISubscriptionService));
            var instance2 = container.GetService(typeof(ISubscriptionService));

            Assert.IsInstanceOf<SubscriptionService>(instance1);
            Assert.AreEqual(instance1, instance2);
        }

        [Test]
        public void Resolves_Controllers()
        {
            var container = ContainerBuilder.GetContainer();

            var queueController = container.GetService(typeof(QueueController));
            var subscriptionController = container.GetService(typeof(SubscribeController));

            Assert.IsNotNull(queueController);
            Assert.IsNotNull(subscriptionController);
        }

        [Test]
        public void Resolves_ISubscriptionTimer()
        {
            var container = ContainerBuilder.GetContainer();

            var instance = container.GetService(typeof(ISubscriptionTimer));

            Assert.IsInstanceOf<SubscriptionTimer>(instance);
        }

        [Test]
        public void Resolves_ISubscribersNotificationAdapter()
        {
            var container = ContainerBuilder.GetContainer();

            var instance = container.GetService(typeof(ISubscribersNotificationAdapter));

            Assert.IsInstanceOf<SubscribersNotificationAdapter>(instance);
        }

        [Test]
        public void Resolves_Persistence_Repository_Using_Songleton()
        {
            var container = ContainerBuilder.GetContainer();

            var instance = container.GetService(typeof(IMessageRepository));
            var instance2 = container.GetService(typeof(IMessageRepository));

            Assert.IsInstanceOf<MessageRepository>(instance);
            Assert.AreEqual(instance2, instance);
        }

        [Test]
        public void Resolves_Repository_If_Persistance_Required()
        {
            PersistanceProvider.PersistMessages = true;
            var persistenceAdapterMock = Substitute.For<IPersistenceAdapter>();
            PersistanceProvider.Adapter = persistenceAdapterMock;
            var container = ContainerBuilder.GetContainer();

            var messageRepository = container.GetService(typeof(IMessageRepository));
            var persistenceAdapterInstance = container.GetService(typeof(IPersistenceAdapter));

            Assert.IsInstanceOf<PersistenceMessageRepository>(messageRepository);
            Assert.AreEqual(persistenceAdapterInstance, persistenceAdapterMock);
        }
    }
}
