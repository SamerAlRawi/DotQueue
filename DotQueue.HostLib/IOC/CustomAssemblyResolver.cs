using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace DotQueue.HostLib.IOC
{
    public class CustomAssemblyResolver : IAssembliesResolver
    {
        public ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly>() {typeof(QueueController).Assembly};
        }
    }
}