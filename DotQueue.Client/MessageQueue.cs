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

        public MessageQueue(DotQueueAddress address)
        {
            _address = address;
            _type = typeof(T).Name;

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
            var postData = BuildMessage(message);
            var request = BuildAddHttpRequest();
            SetHeaders(request, WebRequestMethods.Http.Post);
            PostMessage(request, postData);
            return GetResponseFromServer(request);
        }

        public T Pull()
        {
            var request = BuildPullHttpRequest();
            SetHeaders(request, WebRequestMethods.Http.Get);
            var json = GetResponseFromServer(request);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public int Count()
        {
            var request = BuildCountHttpRequest();
            SetHeaders(request, WebRequestMethods.Http.Get);
            var json = GetResponseFromServer(request);
            return int.Parse(json);
        }

        private HttpWebRequest BuildPullHttpRequest()
        {
            var request =
                WebRequest.Create($"http://{_address.IpAddress}:{_address.Port}/api/Queue/Pull?category={_type}") as
                    HttpWebRequest;
            return request;
        }

        private HttpWebRequest BuildCountHttpRequest()
        {
            var requestUriString = $"http://{_address.IpAddress}:{_address.Port}/api/Queue/Count?category={_type}";
            var request = WebRequest.Create(requestUriString) as HttpWebRequest;
            return request;
        }

        private HttpWebRequest BuildSubscribeHttpRequest(int port)
        {
            var requestUriString =
                $"http://{_address.IpAddress}:{_address.Port}/api/Subscribe/Subscribe?category={_type}&port={port}";
            var request = WebRequest.Create(requestUriString) as HttpWebRequest;
            return request;
        }

        private HttpWebRequest BuildAddHttpRequest()
        {
            var request =
                WebRequest.Create($"http://{_address.IpAddress}:{_address.Port}/api/Queue/Add") as HttpWebRequest;
            return request;
        }

        private string BuildMessage(T message)
        {
            var msg = JsonConvert.SerializeObject(message);
            var wrapper = new Message {Type = _type, Body = msg};
            var postData = JsonConvert.SerializeObject(wrapper);
            return postData;
        }

        private static void PostMessage(HttpWebRequest request, string postData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
        }

        private static string GetResponseFromServer(HttpWebRequest request)
        {
            string responseFromServer;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    responseFromServer = reader.ReadToEnd();
                }
            }
            return responseFromServer;
        }

        private static void SetHeaders(HttpWebRequest request, string method)
        {
            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";
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

        private void SubscribeToQueue(int localPort)
        {
            try
            {
                var request = BuildSubscribeHttpRequest(localPort);
                request.Method = WebRequestMethods.Http.Get;
                request.GetResponse();
            }
            catch (Exception)
            {
            }
        }
    }
}