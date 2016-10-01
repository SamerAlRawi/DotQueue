using System;
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
            var msg = JsonConvert.SerializeObject(message);
            var wrapper = new Message {Type = _type, Body = msg};
            var postData = JsonConvert.SerializeObject(wrapper);
            var request = WebRequest.Create($"http://{_address.IpAddress}:{_address.Port}/api/Queue/Add") as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            var responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }

        public T Pull()
        {
            throw new System.NotImplementedException();
        }
    }
}