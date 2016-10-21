using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace DotQueue.Client
{
    public class DotQueueAddress
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
    }

    public class MessageQueue<T> : IMessageQueue<T>
    {
        private DotQueueAddress _address;
        private string _type;

        public MessageQueue(DotQueueAddress address)
        {
            _address = address;
            _type = typeof(T).Name;
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

        private HttpWebRequest BuildPullHttpRequest()
        {
            var request = WebRequest.Create($"http://{_address.IpAddress}:{_address.Port}/api/Queue/Pull?category={_type}") as HttpWebRequest;
            return request;
        }

        private HttpWebRequest BuildAddHttpRequest()
        {
            var request = WebRequest.Create($"http://{_address.IpAddress}:{_address.Port}/api/Queue/Add") as HttpWebRequest;
            return request;
        }

        private string BuildMessage(T message)
        {
            var msg = JsonConvert.SerializeObject(message);
            var wrapper = new Message { Type = _type, Body = msg };
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
    }
}