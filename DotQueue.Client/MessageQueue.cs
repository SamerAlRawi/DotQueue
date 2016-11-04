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

        public MessageQueue(DotQueueAddress address)
        {
            _httpAdapter = new HttpAdapter<T>(address);
            _address = address;
            _type = typeof(T).Name;
            InitializeQueueTasks();
        }

        internal MessageQueue(DotQueueAddress address, IHttpAdapter<T> httpAdapter)
        {
            _httpAdapter = httpAdapter;
            _address = address;
            _type = typeof(T).Name;
            InitializeQueueTasks();
        }

        private void InitializeQueueTasks()
        {
            Task.Run(() => StartListener());
            Task.Run(() => SubscribeToQueue(_localPort));
            Task.Run(() => ReSubscribe());
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

        private void StartListener()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{_localPort}/");
            listener.Start();
            while (true)
                try
                {
                    {
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        ProcessRequest(request.Url.LocalPath);
                        HttpListenerResponse response = context.Response;
                        string responseString = "OK";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }

                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            listener.Stop();
        }

        private void ProcessRequest(string message)
        {
            Console.WriteLine($"Message received: {message}");
            if (message.Contains("subscribtion_added"))
            {
                _subscribed = true;
                _subscriptionConfirmTime = DateTime.Now;
            }
            if (message.Contains("new_message"))
            {
                _messageFound = true;
            }

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