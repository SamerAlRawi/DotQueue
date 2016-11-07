using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotQueue.HostLib;
using NUnit.Framework;

namespace DotQueue.Repository.Tests
{
    [TestFixture, Category("no_ci"), Explicit]
    public class QueueHostTests
    {
        private QueueHost _host;
        private int _defaultPort = 8088;

        [SetUp]
        public void Setup()
        {
            _host = new QueueHost(_defaultPort);
            Task.Run(() => _host.Start());
            Thread.Sleep(2000);
        }

        [Test]
        public void Api_Is_Running()
        {
            var request = WebRequest.Create($"http://127.0.0.1:{_defaultPort}/api/Queue/AreYouAlive") as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 3000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            string responseFromServer = "";
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream);
                    responseFromServer = reader.ReadToEnd();
                }
            }
            Assert.AreEqual(responseFromServer, "\"YES\"");
        }

        [TearDown]
        public void Cleanup()
        {
            _host.Stop();
        }
    }
}
