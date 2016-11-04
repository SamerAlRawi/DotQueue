using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.Client.Tests
{
    [TestFixture]
    public class MessageQueueTests
    {
        private MessageQueue<Profile> _messageQueue;
        private DotQueueAddress _defaultAddress = new DotQueueAddress();
        private IListenerAdapter<Profile> _listenerAdapter;
        private IHttpAdapter<Profile> _httpAdapter;
        private ILocalPortResolver _portResolver;
        private int _defaultLocalPort = 1122;

        [SetUp]
        public void Setup()
        {
            _portResolver = Substitute.For<ILocalPortResolver>();
            _portResolver.FindFreePort().Returns(_defaultLocalPort);
            _listenerAdapter = Substitute.For<IListenerAdapter<Profile>>();
            _httpAdapter = Substitute.For<IHttpAdapter<Profile>>();
            _messageQueue = new MessageQueue<Profile>(_defaultAddress, 
                _httpAdapter, _listenerAdapter, _portResolver);    
        }

        [Test]
        public void Start_Listener_With_Correct_Port()
        {
            Thread.Sleep(1000);
            _listenerAdapter.Received().StartListener(_defaultLocalPort);
        }

        [Test]
        public void Subscribe_To_Queue()
        {
            Thread.Sleep(1000);
            _httpAdapter.Received().Subscribe(_defaultLocalPort);
        }

        [Test]
        public void Renew_Subscription_After_Period()
        {
            Thread.Sleep(1000);
            _httpAdapter.Received().Subscribe(_defaultLocalPort);
        }
    }
}