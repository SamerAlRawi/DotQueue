using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using DotQueue.HostLib.IOC;
using WebApiContrib.Formatting.Jsonp;

namespace DotQueue.HostLib
{
    public class QueueHost
    {
        private HttpSelfHostServer _httpSelfHostServer;
        private int _port;
        private IApiTokenValidator _tokenValidator;

        public QueueHost(int port, IApiTokenValidator tokenValidator = null)
        {
            _tokenValidator = tokenValidator;
            _port = port;
            ConfigureAuthentication();
        }

        private void ConfigureAuthentication()
        {
            if (_tokenValidator != null)
            {
                TokenValidationProvider.CheckAuthorization = true;
                TokenValidationProvider.Validator = _tokenValidator;
            }
        }

        public void Start()
        {
            StartApiHost();
        }
        
        private void StartApiHost()
        {
            var configuration = new HttpSelfHostConfiguration($"http://0.0.0.0:{_port}");
            configuration.Routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            configuration.Routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            configuration.Routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            configuration.Routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver());
            configuration.DependencyResolver = ContainerBuilder.GetContainer();
            configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter(new JsonMediaTypeFormatter(), "callback"));
            //configuration.Services.Replace(typeof(IExceptionHandler), new DebuggingExceptionHandler());
            _httpSelfHostServer = new HttpSelfHostServer(configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _httpSelfHostServer.CloseAsync();
            _httpSelfHostServer.Dispose();
        }
    }

    internal class DebuggingExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}