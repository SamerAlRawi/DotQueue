using DotQueue.Api;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace DotQueue.HostLib
{
    public class CustomAssemblyResolver : IAssembliesResolver
    {
        public CustomAssemblyResolver()
        {
        }

        public ICollection<Assembly> GetAssemblies()
        {
            return new List<Assembly>() {typeof(MainController).Assembly};
        }
    }
}