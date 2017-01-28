using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using DotQueue.HostLib.IOC;

namespace DotQueue.HostLib
{
    public class ApiHostConfigurationBuilder
    {
        public HttpSelfHostConfiguration BuildHostConfiguration(int port, long maxMessageSize)
        {
            var configuration = new HttpSelfHostConfiguration($"http://0.0.0.0:{port}");
            configuration.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new {id = RouteParameter.Optional},
                new {id = @"\d+"});
            configuration.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            configuration.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new {action = "Get"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Get)});
            configuration.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new {action = "Post"},
                new {httpMethod = new HttpMethodConstraint(HttpMethod.Post)});
            configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver());
            configuration.DependencyResolver = ContainerBuilder.GetContainer();
            configuration.MaxReceivedMessageSize = maxMessageSize;
#if DEBUG
            configuration.Services.Replace(typeof(IExceptionHandler), new DebuggingExceptionHandler());
#endif
            return configuration;
        }
    }
}