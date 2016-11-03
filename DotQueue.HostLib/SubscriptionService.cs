using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
                if (client.LastNotified < DateTime.Now.Subtract(TimeSpan.FromMinutes(1)) || _messageRepository.Count(category) == 1)
                {
                    client.LastNotified = DateTime.Now;
                    Notify(client, "new_message");
                }
            }
        }

        private void Notify(ClientAddress client, string message)
        {
            try
            {
                var request = WebRequest.Create($"http://{client.IpAddress}:{client.Port}/{message}");
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
            var address = new ClientAddress { Category = category, Port = port, IpAddress = clientAddress };
            if (!_clients.Contains(address))
            {
                _clients.Add(address);
            }
            Task.Run(() => Notify(address, "subscribtion_added"));
        }
    }
}