using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.Client.Tests
{
    [TestFixture, Explicit, Category("no_ci")]
    public class HttpAdapterTests
    {
        private HttpAdapter<Profile> _adapter;
        private IJsonSerializer<Profile> _serializer;
        private DotQueueAddress _defaultAddress;
        List<HttpListenerRequest> _requests = new List<HttpListenerRequest>();
        private bool _listen;
        private string _applicationJsonHeader = "application/json";
        private string _mockResponse;

        [SetUp]
        public void Setup()
        {
            _requests.Clear();
            _mockResponse = "OK";
            _serializer = Substitute.For<IJsonSerializer<Profile>>();
            _defaultAddress = new DotQueueAddress { IpAddress = IPAddress.Parse("127.0.0.1"), Port = 8083 };
            Task.Run(() => StartMockListener());
            _adapter = new HttpAdapter<Profile>(_defaultAddress, _serializer);
            Thread.Sleep(500);//guarantee tcp port opened
        }

        [Test]
        public void Add_Send_Json_Message_To_Queue()
        {
            var profile = new Profile();
            string expectedJson = "{test: 233}";
            var expectedId = Guid.NewGuid().ToString();
            _mockResponse = expectedId;
            _serializer.Serialize(profile).Returns(expectedJson);
            var actualId = _adapter.Add(profile);

            _serializer.Received().Serialize(profile);
            Assert.AreEqual(_requests.First().Headers["Content-Type"], _applicationJsonHeader);
            Assert.AreEqual(_requests.First().Headers["Accept"], _applicationJsonHeader);
            Assert.That(_requests.First().Url.LocalPath, Is.EqualTo("/api/Queue/Add"));
            Assert.That(_requests.First().HttpMethod, Is.EqualTo("POST"));
            Assert.AreEqual(actualId, expectedId);
        }

        [Test]
        public void Pull_Sets_Category_And_Deserialize_And_Return_Item()
        {
            var profile = new Profile();
            _mockResponse = "{test: 233}";
            _serializer.Deserialize(_mockResponse).Returns(profile);

            var actual = _adapter.Pull();

            var request = _requests.First();
            Assert.AreEqual(request.Headers["Content-Type"], _applicationJsonHeader);
            Assert.AreEqual(request.Headers["Accept"], _applicationJsonHeader);
            Assert.That(request.Url.LocalPath, Is.EqualTo("/api/Queue/Pull"));
            Assert.AreEqual(request.Url.Query, $"?category={typeof(Profile).Name}");
            Assert.That(request.HttpMethod, Is.EqualTo("GET"));
            Assert.AreEqual(actual, profile);
        }

        [Test]
        public void Count_Request_Include_Category()
        {
            _mockResponse = "1233";

            var actual = _adapter.Count();

            var request = _requests.First();
            Assert.AreEqual(request.Headers["Content-Type"], _applicationJsonHeader);
            Assert.AreEqual(request.Headers["Accept"], _applicationJsonHeader);
            Assert.That(request.Url.LocalPath, Is.EqualTo("/api/Queue/Count"));
            Assert.AreEqual(request.Url.Query, $"?category={typeof(Profile).Name}");
            Assert.That(request.HttpMethod, Is.EqualTo("GET"));
            Assert.AreEqual(actual, int.Parse(_mockResponse));
        }

        [Test]
        public void Subscribe_Request_Include_Category_And_Port()
        {
            var localPort = 2323;
            var actual = _adapter.Subscribe(localPort);

            var request = _requests.First();
            Assert.That(request.Url.LocalPath, Is.EqualTo("/api/Subscribe/Subscribe"));
            Assert.AreEqual(request.Url.Query, $"?category={typeof(Profile).Name}&port={localPort}");
            Assert.That(request.HttpMethod, Is.EqualTo("GET"));
        }

        [TearDown]
        public void Clear()
        {
            StopMockListener();
        }
        private void StartMockListener()
        {
            _listen = true;
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{_defaultAddress.Port}/");
            listener.Start();

            while (_listen)
                try
                {
                    {
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        _requests.Add(request);
                        HttpListenerResponse response = context.Response;
                        byte[] buffer = Encoding.UTF8.GetBytes(_mockResponse);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
                catch (Exception)
                {

                }
            listener.Stop();
            listener.Close();

        }
        private void StopMockListener()
        {
            _listen = false;
            Thread.Sleep(500);//Guarantee listener port is released
        }
    }
}