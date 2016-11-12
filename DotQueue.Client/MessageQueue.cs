using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DotQueue.Client
{
    /// <summary>
    /// Generic queue class to send messages to queue and subscribe and listen for new messages of the type "T"
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        private IApiTokenSource _tokenSource;

        public MessageQueue(DotQueueAddress address, IApiTokenSource tokenSource = null)
        {
            _tokenSource = tokenSource;
            _httpAdapter = new HttpAdapter<T>(address, new JsonSerializer<T>(), new JsonSerializer<Message>(), _tokenSource);
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

        /// <summary>
        /// sends message to DotQueue queue
        /// </summary>
        /// <param name="message"></param>
        /// <returns>string: a unique identifier of the received message</returns>
        public string Add(T message)
        {
            if(message == null)
                throw new ArgumentNullException("message");
            return _httpAdapter.Add(message);
        }
        /// <summary>
        /// pulls message from remote queue
        /// </summary>
        /// <returns>a message form the queue</returns>
        public T Pull()
        {
            return _httpAdapter.Pull();
        }
        /// <summary>
        /// Returns the count of messages in queue
        /// </summary>
        /// <returns>int: the count of messages of type(T) currently queued in the remote queue</returns>
        public int Count()
        {
            return _httpAdapter.Count();
        }
        /// <summary>
        /// Endless enumeration of messages, subscribe and pull messages and wait for new messages if queue is empty
        /// </summary>
        /// <returns></returns>
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
            new Thread(() => SubscribeToQueue(_localPort)).Start();
            new Thread(ReSubscribe).Start();
            _listenerAdapter.NotificationReceived += ProcessNotification;
        }

        private void ProcessNotification(object sender, QueueNotification e)
        {
            #if DEBUG
            Console.WriteLine(e);
            #endif
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