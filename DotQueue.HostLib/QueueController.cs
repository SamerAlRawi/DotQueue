using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DotQueue.HostLib
{
    public class QueueController : ApiController
    {
        private IMessageRepository _repository;

        public QueueController(IMessageRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost]
        public string Add([FromBody]Message message)
        {
            var id = Guid.NewGuid().ToString();
            message.Id = id;
            Task.Run(() => _repository.Add(message));
            return id;
        }

        [HttpGet]
        public Message Pull([FromUri]string category)
        {
            return _repository.Pull(category);
        }

        [HttpGet]
        public int Count([FromUri]string category)
        {
            return _repository.Count(category);
        }

        [HttpGet]
        public string AreYouAlive()
        {
            return "OK";
        }
    }
}

