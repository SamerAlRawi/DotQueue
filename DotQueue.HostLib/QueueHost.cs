using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using DotQueue.HostLib.IOC;

namespace DotQueue.HostLib
{
    /// <summary>
    /// class for hosting DotQueue queue process
    /// </summary>
    public class QueueHost
    {
        private HttpSelfHostServer _httpSelfHostServer;
        private int _port;
        private IApiTokenValidator _tokenValidator;
        private long _maxMessageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">http port fir queue listener</param>
        /// <param name="tokenValidator">token validation, if specified, queue will require client to send a valid token</param>
        /// <param name="maxMessageSize">message size in bytes, override if you need to restrict message size</param>
        public QueueHost(int port, IApiTokenValidator tokenValidator = null, int maxMessageSize = 65536)
        {
            _maxMessageSize = maxMessageSize;
            _tokenValidator = tokenValidator;
            _port = port;
            ConfigureAuthentication();
        }
        /// <summary>
        /// Start the queue listener
        /// </summary>
        /// <exception cref="System.ServiceModel.AddressAccessDeniedException">thrown if process identity is not permitted to open http ports</exception>
        /// <exception cref="System.AggregateException">thrown if port is already in use</exception>
        public void Start()
        {
            StartApiHost();
        }
        /// <summary>
        /// stop queue listener
        /// </summary>
        public void Stop()
        {
            if (_httpSelfHostServer != null)
            {
                _httpSelfHostServer.CloseAsync();
                _httpSelfHostServer.Dispose();
            }
        }
        private void ConfigureAuthentication()
        {
            if (_tokenValidator != null)
            {
                TokenValidationProvider.CheckAuthorization = true;
                TokenValidationProvider.Validator = _tokenValidator;
            }
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
            configuration.MaxReceivedMessageSize = _maxMessageSize;
#if DEBUG
            configuration.Services.Replace(typeof(IExceptionHandler), new DebuggingExceptionHandler());
#endif
            _httpSelfHostServer = new HttpSelfHostServer(configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }


    }

    internal class DebuggingExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            throw context.Exception;
        }
    }
}