using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.SelfHost;

namespace DotQueue.HostLib
{
    /// <summary>
    /// class for hosting DotQueue queue process
    /// </summary>
    public class QueueHost
    {
        private HttpSelfHostServer _httpSelfHostServer;
        private readonly int _port;
        private readonly IApiTokenValidator _tokenValidator;
        private readonly long _maxMessageSize;
        private readonly IPersistenceAdapter _persistenceAdapter;
        private readonly ApiHostConfigurationBuilder _apiHostConfigurationBuilder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port">http port fir queue listener</param>
        /// <param name="tokenValidator">token validation, if specified, queue will require client to send a valid token</param>
        /// <param name="persistenceAdapter">persistance adapter, queue will persist messages if thie argument is specified</param>
        /// <param name="maxMessageSize">message size in bytes, override if you need to restrict message size</param>
        public QueueHost(int port, IApiTokenValidator tokenValidator = null, IPersistenceAdapter persistenceAdapter = null,
            int maxMessageSize = 65536)
        {
            _persistenceAdapter = persistenceAdapter;
            _maxMessageSize = maxMessageSize;
            _tokenValidator = tokenValidator;
            _port = port;
            ConfigureAuthentication();
            ConfigurePersistance();
            _apiHostConfigurationBuilder = new ApiHostConfigurationBuilder();
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

        private void ConfigurePersistance()
        {
            if (_persistenceAdapter != null)
            {
                PersistanceProvider.PersistMessages = true;
                PersistanceProvider.Adapter = _persistenceAdapter;
            }
        }

        private void StartApiHost()
        {
            var configuration = _apiHostConfigurationBuilder.BuildHostConfiguration(_port, _maxMessageSize);
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