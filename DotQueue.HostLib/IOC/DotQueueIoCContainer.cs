using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace DotQueue.HostLib.IOC
{
    internal class DotQueueIoCContainer : DotQueueScopeContainer, IDependencyResolver
    {
        public DotQueueIoCContainer(IUnityContainer container) : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            var child = _container.CreateChildContainer();
            return new DotQueueScopeContainer(child);
        }
    }
}