using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DotQueue.Client.Tests
{
    [TestFixture, Explicit]
    [Category("no_ci")]
    public class ListenerAdapterTests
    {
        private ListenerAdapter<Profile> _adapter;

        [SetUp]
        public void Setup()
        {
            _adapter = new ListenerAdapter<Profile>();
        }

        [Test]
        public async Task StartListener_Start_Http_Listener()
        {
            var port = 8023;
            _adapter.StartListener(port);

            HttpClient client = new HttpClient();
            var result = await client.GetAsync($"http://127.0.0.1:{port}/");
            Assert.That(await result.Content.ReadAsStringAsync(), Is.EqualTo("OK"));
        }

        [Test]
        public async Task Triggers_NewMessage_If_Message_Reeived()
        {
            var newMessageInvoked = false;
            var port = 8024;
            _adapter.StartListener(port);
            _adapter.NotificationReceived += (sender, notification) =>
            {
                newMessageInvoked = notification == QueueNotification.NewMessage;
            };
            HttpClient client = new HttpClient();
            var result = await client.GetAsync($"http://127.0.0.1:{port}/new_message");
            var response = result.Content.ReadAsStringAsync();
            Thread.Sleep(500);
            Assert.IsTrue(newMessageInvoked);
        }

        [Test]
        public async Task Triggers_SubscriptionConfirmed_If_Message_Reeived()
        {
            var subscriptionConfirm = false;
            var port = 8025;
            _adapter.StartListener(port);
            _adapter.NotificationReceived += (sender, notification) =>
            {
                subscriptionConfirm = notification == QueueNotification.SubscriptionConfirmed;
            };
            HttpClient client = new HttpClient();
            var result = await client.GetAsync($"http://127.0.0.1:{port}/subscribtion_added");
            var response = result.Content.ReadAsStringAsync();
            Thread.Sleep(500);
            Assert.IsTrue(subscriptionConfirm);
        }

        [TearDown]
        public void Clear()
        {
            _adapter.Dispose();
        }
    }
}
