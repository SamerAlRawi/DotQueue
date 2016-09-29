using System.Web.Http;

namespace DotQueue.Api
{
    public class ChartController : ApiController
    {
        [HttpGet]
        public ChartInfo GetByHours()
        {
            return new ChartInfo
            {
                Labels = new []{"Jan", "February", "March", "April", "May", "June", "July"},
                Messages = new decimal[]{65, 59, 80, 81, 56, 55, 40},
                Pulls = new decimal[]{62, 60, 77, 79, 55, 55, 38}
            };
        }
    }
}