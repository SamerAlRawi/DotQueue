using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotQueue.Client
{
    public class MessageQueue<T> : IMessageQueue<T>
    {
        private int _localPort;
        private bool _messageFound;
        private bool _subscribed;
        private DateTime _subscriptionConfirmTime = DateTime.MinValue;
        private IHttpAdapter<T> _httpAdapter;
        private IListenerAdapter<T> _listenerAdapter;
        private ILocalPortResolver _portResolver;
        private IWaitDurationHelper _durationHelper;

        public MessageQueue(DotQueueAddress address)
        {
            _httpAdapter = new HttpAdapter<T>(address);
            _listenerAdapter = new ListenerAdapter<T>();
            _portResolver = new LocalPortResolver();
            _durationHelper = new WaitDurationHelper();
            InitializeQueueTasks(address);
        }

        internal MessageQueue(DotQueueAddress address, 
            IHttpAdapter<T> httpAdapter, 
            IListenerAdapter<T> listenerAdapter,
            ILocalPortResolver portResolver, 
            IWaitDurationHelper durationHelper)
        {
            _durationHelper = durationHelper;
            _portResolver = portResolver;
            _listenerAdapter = listenerAdapter;
            _httpAdapter = httpAdapter;
            InitializeQueueTasks(address);
        }

        public string Add(T message)
        {
            return _httpAdapter.Add(message);
        }

        public T Pull()
        {
            return _httpAdapter.Pull();
        }

        public int Count()
        {
            return _httpAdapter.Count();
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (true)
            {
                if (Count() > 0)
                {
                    yield return Pull();
                }
                else
                {
                    _messageFound = false;
                    WaitForNewMessage();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void InitializeQueueTasks(DotQueueAddress address)
        {
            _localPort = _portResolver.FindFreePort();
            _listenerAdapter.StartListener(_localPort);
            Task.Run(() => SubscribeToQueue(_localPort));
            Task.Run(() => ReSubscribe());
            _listenerAdapter.NotificationReceived += ProcessNotification;
        }

        private void ProcessNotification(object sender, QueueNotification e)
        {
            Console.WriteLine($"Message received: {e}");
            if (e == QueueNotification.SubscriptionConfirmed)
            {
                _subscribed = true;
                _subscriptionConfirmTime = DateTime.Now;
            }
            if (e == QueueNotification.NewMessage)
            {
                _messageFound = true;
            }

        }

        private void ReSubscribe()
        {
            while (true)
            {
                Thread.Sleep(_durationHelper.SubscribtionRenewalSpan());
                if (_subscriptionConfirmTime < DateTime.Now.Subtract(_durationHelper.SubscribtionRenewalSpan()))
                {
                    SubscribeToQueue(_localPort);
                }
            }
        }

        private void WaitForNewMessage()
        {
            while (!_messageFound)
            {
                Thread.Sleep(_durationHelper.NewMessageWaitDuration());
            }
        }

        private bool SubscribeToQueue(int localPort)
        {
            return _httpAdapter.Subscribe(localPort);
        }
    }
}