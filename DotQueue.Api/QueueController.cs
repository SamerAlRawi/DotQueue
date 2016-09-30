using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using DotQueue.Domain;
using DotQueue.Repository;

namespace DotQueue.Api
{
    public class QueueController : ApiController
    {
        private IMessageRepository _repository;

        public QueueController(IMessageRepository repository)
        {
            _repository = repository;
        }
        
        [HttpPost]
        public string Add([FromBody]string message)
        {
            var id = Guid.NewGuid().ToString();
            _repository.Add(new Message
            {
                Id = id,
                Body = message
            });
            return id;
        }

        [HttpGet]
        public Message Pull()
        {
            return _repository.Pull();
        }
    }
}

