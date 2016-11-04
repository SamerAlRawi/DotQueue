using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DotQueue.Client
{
    public class MessageQueue<T> : IMessageQueue<T>
    {
        private DotQueueAddress _address;
        private string _type;
        private int _localPort = 8082;
        private bool _messageFound = false;
        private bool _subscribed = false;
        private DateTime _subscriptionConfirmTime = DateTime.MinValue;
        private IHttpAdapter<T> _httpAdapter;
        private IListenerAdapter<T> _listenerAdapter;

        public MessageQueue(DotQueueAddress address)
        {
            _httpAdapter = new HttpAdapter<T>(address);
            _listenerAdapter = new ListenerAdapter<T>();
            InitializeQueueTasks(address);
        }

        internal MessageQueue(DotQueueAddress address, 
            IHttpAdapter<T> httpAdapter, IListenerAdapter<T> listenerAdapter)
        {

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
            _address = address;
            _type = typeof(T).Name;
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

        private Action ReSubscribe()
        {
            while (true)
            {
                Thread.Sleep(5000);
                if (_subscriptionConfirmTime < DateTime.Now.Subtract(TimeSpan.FromSeconds(60)))
                {
                    SubscribeToQueue(_localPort);
                }
            }
        }

        private void WaitForNewMessage()
        {
            while (!_messageFound)
            {
                Thread.Sleep(100);
            }
        }

        private bool SubscribeToQueue(int localPort)
        {
            return _httpAdapter.Subscribe(localPort);
        }
    }
}