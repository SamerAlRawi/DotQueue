using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace DotQueue.HostLib.IOC
{
    public static class ContainerBuilder
    {
        public static IDependencyResolver GetContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IMessageRepository, MessageRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<QueueController>(
                new InjectionFactory(_ => new QueueController(_.Resolve<IMessageRepository>())));
            return new DotQueueIoCContainer(container);
        }
    }
}
