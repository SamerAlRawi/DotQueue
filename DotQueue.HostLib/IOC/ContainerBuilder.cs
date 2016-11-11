using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace DotQueue.HostLib.IOC
{
    public static class ContainerBuilder
    {
        public static IDependencyResolver GetContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<ISubscribersNotificationAdapter, SubscribersNotificationAdapter>();
            container.RegisterType<ISubscriptionTimer, SubscriptionTimer>();
            container.RegisterType<ISubscriptionService, SubscriptionService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMessageRepository, MessageRepository>(new ContainerControlledLifetimeManager());

            container.RegisterType<QueueController>(
                new InjectionFactory(_ => new QueueController(_.Resolve<IMessageRepository>())));

            container.RegisterType<SubscribeController>(
                            new InjectionFactory(_ => new SubscribeController(_.Resolve<ISubscriptionService>())));

            return new DotQueueIoCContainer(container);
        }
    }
}
