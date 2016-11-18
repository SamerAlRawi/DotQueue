using System;
using System.IO;
using System.Net;
using System.Text;

namespace DotQueue.Client
{
    internal class HttpAdapter<T> : IHttpAdapter<T>
    {
        private readonly DotQueueAddress _address;
        private readonly string _type;
        private readonly IJsonSerializer<T> _serializer;
        private readonly IJsonSerializer<Message> _messageSerializer;
        private readonly IApiTokenSource _tokenSource;

        public HttpAdapter(DotQueueAddress address, IJsonSerializer<T> serializer, IJsonSerializer<Message> messageSerializer, IApiTokenSource tokenSource = null)
        {
            _tokenSource = tokenSource;
            _messageSerializer = messageSerializer;
            _address = address;
            _type = typeof(T).Name;
            _serializer = serializer;
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
            var message = _messageSerializer.Deserialize(json);
            return _serializer.Deserialize(message.Body.Base64Decode());
        }

        public int Count()
        {
            var request = BuildCountHttpRequest();
            SetHeaders(request, WebRequestMethods.Http.Get);
            var json = GetResponseFromServer(request);
            return int.Parse(json);
        }

        public bool Subscribe(int localPort)
        {
            try
            {
                SubscribeToQueue(localPort);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private HttpWebRequest BuildPullHttpRequest()
        {
            var requestUriString = $"http://{_address.IpAddress}:{_address.Port}/api/Queue/Pull?category={_type}";
            return BuildRequest(requestUriString);
        }

        private HttpWebRequest BuildCountHttpRequest()
        {
            var requestUriString = $"http://{_address.IpAddress}:{_address.Port}/api/Queue/Count?category={_type}";
            return BuildRequest(requestUriString);
        }
        
        private HttpWebRequest BuildAddHttpRequest()
        {
            var requestUriString = $"http://{_address.IpAddress}:{_address.Port}/api/Queue/Add";
            return BuildRequest(requestUriString);
        }

        private string BuildMessage(T message)
        {
            var msg = _serializer.Serialize(message);
            var wrapper = new Message { Type = _type, Body = msg.Base64Encode() };
            var postData = _serializer.Serialize(wrapper);
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

        private void SubscribeToQueue(int port)
        {
            var requestUriString = $"http://{_address.IpAddress}:{_address.Port}/api/Subscribe/Subscribe?category={_type}&port={port}";
            var request = BuildRequest(requestUriString);
            request.Method = WebRequestMethods.Http.Get;
            request.GetResponse();
        }

        private HttpWebRequest BuildRequest(string requestUriString)
        {
            return WebRequest.Create(requestUriString) as HttpWebRequest;
        }

        private void SetHeaders(HttpWebRequest request, string method)
        {
            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            if (_tokenSource != null)
            {
                request.Headers.Add("Api-Token", _tokenSource.GetToken());
            }
        }

    }
}