using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DotQueue.HostLib
{
    internal class SubscriptionService : ISubscriptionService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ConcurrentBag<Subscriber> _subscribers = new ConcurrentBag<Subscriber>();
        private readonly ISubscribersNotificationAdapter _notificationAdapter;
        private readonly ISubscriptionTimer _timer;

        public SubscriptionService(IMessageRepository messageRepository, 
            ISubscribersNotificationAdapter notificationAdapter, ISubscriptionTimer timer)
        {
            _timer = timer;
            _messageRepository = messageRepository;
            _messageRepository.NewMessage += TellSubscribers;
            _notificationAdapter = notificationAdapter;
        }
        private void TellSubscribers(object sender, string category)
        {
            foreach (var client in _subscribers.Where(c => c.Category == category))
            {
                if (DueforNotification(category, client))
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
        private bool DueforNotification(string category, Subscriber client)
        {
            var leaseTimedout = client.LastNotified < DateTime.UtcNow.Subtract(_timer.RenewalInterval());
            var haveMessagesInQueue = _messageRepository.Count(category) == 1;

            return leaseTimedout || haveMessagesInQueue;
        }
    }
}