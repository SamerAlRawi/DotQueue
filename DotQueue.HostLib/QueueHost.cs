using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using DotQueue.HostLib.IOC;
using WebApiContrib.Formatting.Jsonp;

namespace DotQueue.HostLib
{
    public class QueueHost
    {
        private HttpSelfHostServer _httpSelfHostServer;
        private IDisposable _staticFileHost;
        private int _port;

        public QueueHost(int port)
        {
            _port = port;
        }

        public void Start()
        {
            StartApiHost();
        }
        
        private void StartApiHost()
        {
            HttpSelfHostConfiguration _configuration = new HttpSelfHostConfiguration($"http://0.0.0.0:{_port}");
            _configuration.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            _configuration.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            _configuration.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            _configuration.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            _configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver());
            _configuration.DependencyResolver = ContainerBuilder.GetContainer();
            _configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter(new JsonMediaTypeFormatter(), "callback"));
            
            _httpSelfHostServer = new HttpSelfHostServer(_configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _httpSelfHostServer.CloseAsync();
            _httpSelfHostServer.Dispose();
        }
    }
}