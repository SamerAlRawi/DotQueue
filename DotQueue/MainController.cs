using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace MyNamespace
{
    public class Message
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
    }

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

        public IEnumerable<Message> GetProductsByCategory(string category)
        {
            return _messages.Where(p => string.Equals(p.Category, category,
                StringComparison.OrdinalIgnoreCase));
        }
    }
}