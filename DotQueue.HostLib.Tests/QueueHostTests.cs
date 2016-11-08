using System.IO;
using System.Net;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture, Category("no_ci")]
    public class QueueHostTests
    {
        private QueueHost _host;
        private int _defaultPort = 8088;

        [Test]
        public void Api_Is_Running()
        {
            _host = new QueueHost(_defaultPort);
            new Thread(_host.Start).Start();
            Thread.Sleep(2000);

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
        
        [Test]
        public void Http401_If_No_Token_specified()
        {
            var tokenValidator = Substitute.For<IApiTokenValidator>();
            string tokenValue = "token_123422";
            tokenValidator.IsValidToken(tokenValue).Returns(true);

            _host = new QueueHost(_defaultPort, tokenValidator);
            new Thread(_host.Start).Start();
            Thread.Sleep(2000);

            var request = WebRequest.Create($"http://127.0.0.1:{_defaultPort}/api/Queue/AreYouAlive") as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 3000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            
            HttpStatusCode responseCode = HttpStatusCode.Accepted;
            try
            {
                request.GetResponse();
            }
            catch (WebException ex)
            {
                responseCode = (ex.Response as HttpWebResponse).StatusCode;
            }
            Assert.AreEqual(HttpStatusCode.Unauthorized, responseCode);
        }

        [Test]
        public void Token_Is_required_If_TokenValidation_specified()
        {
            var tokenValidator = Substitute.For<IApiTokenValidator>();
            string tokenValue = "token_123422";
            tokenValidator.IsValidToken(tokenValue).Returns(true);

            _host = new QueueHost(_defaultPort, tokenValidator);
            new Thread(_host.Start).Start();
            Thread.Sleep(2000);

            var request = WebRequest.Create($"http://127.0.0.1:{_defaultPort}/api/Queue/AreYouAlive") as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 3000;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Api-Token", tokenValue);
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
        
        [Test]
        public void TokenProvider_Is_Initialized_If_tokenValidation_specified()
        {
            var tokenValidator = Substitute.For<IApiTokenValidator>();
            _host = new QueueHost(_defaultPort, tokenValidator);
            Thread.Sleep(1000);
            Assert.IsTrue(TokenValidationProvider.CheckAuthorization);
            Assert.AreEqual(tokenValidator, TokenValidationProvider.Validator);
        }
        
        [TearDown]
        public void Cleanup()
        {
            _host.Stop();
        }
    }
}
