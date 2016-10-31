using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace DotQueue.HostLib.IOC
{
    internal class DotQueueScopeContainer : IDependencyScope
    {
        protected IUnityContainer _container;

        internal DotQueueScopeContainer(IUnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            if (_container.IsRegistered(serviceType))
            {
                return _container.Resolve(serviceType);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_container.IsRegistered(serviceType))
            {
                return _container.ResolveAll(serviceType);
            }
            else
            {
                return new object[0];
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}