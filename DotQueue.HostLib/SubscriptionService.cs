using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;

namespace DotQueue.HostLib
{
    public class SubscriptionService : ISubscriptionService
    {
        private IMessageRepository _messageRepository;
        private ConcurrentBag<ClientAddress> _clients = new ConcurrentBag<ClientAddress>();

        public SubscriptionService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            _messageRepository.NewMessage += TellSubscribers;
        }

        private void TellSubscribers(object sender, string category)
        {
            foreach (var client in _clients.Where(c => c.Category == category))
            {
                Notify(client);
            }
        }

        private void Notify(ClientAddress client)
        {
            try
            {
                var request = WebRequest.Create($"http://{client.IpAddress}:{client.Port}");
                request.Method = WebRequestMethods.Http.Get;
                request.GetResponse();
            }
            catch (Exception)
            {
                //TODO logging or remove subscriber!
            }
        }

        public void Subscribe(string clientAddress, int port, string category)
        {
            _clients.Add(new ClientAddress { Category = category, Port = port, IpAddress = clientAddress });
        }
    }

    class ClientAddress
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
    }
}