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
        public string Add([FromBody]Message message)
        {
            var id = Guid.NewGuid().ToString();
            message.Id = id;
            _repository.Add(message);
            return id;
        }

        [HttpGet]
        public Message Pull([FromUri]Category category)
        {
            return _repository.Pull();
        }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}

