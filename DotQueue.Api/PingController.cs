using System.Web.Http;

namespace DotQueue.Api
{
    public class PingController : ApiController
    {
        [HttpGet]
        public string AreYouAlive()
        {
            return "OK";
        }
    }
}