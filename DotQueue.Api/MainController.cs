using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace DotQueue.Api
{
    public class MainController : ApiController
    {
        Message[] _messages = new Message[]
        {
            new Message {Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1},
            new Message {Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M},
            new Message {Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M}
        };

        public IEnumerable<Message> GetAllMessages()
        {
            return _messages;
        }

        public Message GetProductById(int id)
        {
            var product = _messages.FirstOrDefault((p) => p.Id == id);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product;
        }
    }
}