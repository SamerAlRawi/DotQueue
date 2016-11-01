using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;

namespace DotQueue.HostLib
{
    public class SubscribeController : ApiController
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscribeController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public void Subscribe([FromUri] string category, [FromUri] int port)
        {
            var ip = GetIpAddress();
            _subscriptionService.Subscribe(ip, port, category);
        }

        private string GetIpAddress()
        {
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            if (Request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            return null;
        }
    }
}