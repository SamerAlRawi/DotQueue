using System;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class SubscriptionServiceTests
    {
        private SubscriptionService _subscriptionService;
        private ISubscriptionTimer _timer;
        private ISubscribersNotificationAdapter _notificationAdapter;
        private IMessageRepository _repository;
        private string _clientAddress = "192.168.1.1";
        private int _port = 8080;
        private string _category = "MessageType";
        private readonly TimeSpan _defaultTimeSpan = TimeSpan.FromMilliseconds(100);
        private readonly string _notificationMessageText = "new_message";
        private readonly string _subscriptionConfirmedMessage = "subscribtion_added";

        [SetUp]
        public void Setup()
        {
            _timer = Substitute.For<ISubscriptionTimer>();
            _timer.RenewalInterval().Returns(_defaultTimeSpan);
            _notificationAdapter = Substitute.For<ISubscribersNotificationAdapter>();
            _repository = Substitute.For<IMessageRepository>();

            _subscriptionService = new SubscriptionService(_repository, _notificationAdapter, _timer);
        }

        [Test]
        public void Confirm_Subscribtion()
        {
            _subscriptionService.Subscribe(_clientAddress, _port, _category);

            Thread.Sleep(_defaultTimeSpan);
            _notificationAdapter.Received().Notify(Arg.Any<Subscriber>(), _subscriptionConfirmedMessage);
        }

        [Test]
        public void Notify_Subscriber_If_Matching_Message_Type_Added()
        {
            _subscriptionService.Subscribe(_clientAddress, _port, _category);
            _repository.NewMessage += Raise.Event<EventHandler<string>>(this, _category);

            Thread.Sleep(_defaultTimeSpan);
            _notificationAdapter.Received().Notify(Arg.Is<Subscriber>(s =>
                s.IpAddress == _clientAddress &&
                s.Port == _port &&
                s.Category == _category), _notificationMessageText);
        }

        [Test]
        public void DoesNot_Notify_Subscriber_If_Message_Type_Is_Not_Matching()
        {
            _subscriptionService.Subscribe(_clientAddress, _port, _category);
            _repository.NewMessage += Raise.Event<EventHandler<string>>(this, "anotherCategory");

            Thread.Sleep(_defaultTimeSpan);
            _notificationAdapter.DidNotReceive().Notify(Arg.Any<Subscriber>(), _notificationMessageText);
        }

        [Test]
        public void DoesNot_Notify_Subscriber_If_LeaseTime_Didnot_Expire()
        {
            _subscriptionService.Subscribe(_clientAddress, _port, _category);
            _repository.NewMessage += Raise.Event<EventHandler<string>>(this, _category);

            _notificationAdapter.Received(1).Notify(Arg.Any<Subscriber>(), _notificationMessageText);
        }
    }
}
