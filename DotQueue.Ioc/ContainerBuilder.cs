using System.Web.Http.Dependencies;
using DotQueue.Repository;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace DotQueue.Ioc
{
    public static class ContainerBuilder
    {
        public static IDependencyResolver GetContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IMessageRepository, MessageRepository>(new ContainerControlledLifetimeManager());
            return new UnityDependencyResolver(container);
        }
    }

}
