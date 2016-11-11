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
        private ConcurrentBag<Subscriber> _subscribers = new ConcurrentBag<Subscriber>();
        private readonly ISubscribersNotificationAdapter _notificationAdapter;

        public SubscriptionService(IMessageRepository messageRepository, ISubscribersNotificationAdapter notificationAdapter)
        {
            _messageRepository = messageRepository;
            _messageRepository.NewMessage += TellSubscribers;
            _notificationAdapter = notificationAdapter;
        }

        private void TellSubscribers(object sender, string category)
        {
            foreach (var client in _subscribers.Where(c => c.Category == category))
            {
                if (client.LastNotified < DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(10)) || _messageRepository.Count(category) == 1)
                {
                    client.LastNotified = DateTime.UtcNow;
                    _notificationAdapter.Notify(client, "new_message");
                }
            }
        }

        public void Subscribe(string clientAddress, int port, string category)
        {
            var address = new Subscriber { Category = category, Port = port, IpAddress = clientAddress };
            if (!_subscribers.Contains(address))
            {
                _subscribers.Add(address);
            }
            Task.Run(() => _notificationAdapter.Notify(address, "subscribtion_added"));
        }
    }
}