using System;
using System.Linq;
using System.Threading;
using DotQueue.HostLib;
using NUnit.Framework;

namespace DotQueue.Repository.Tests
{
    [TestFixture]
    public class MessageRepositoryTests
    {
        MessageRepository _repository;
        private string _messageType;

        [SetUp]
        public void Setup()
        {
            _repository = new MessageRepository();
            _messageType = typeof(DateTime).ToString();
        }

        [Test]
        public void Add_Adds_Message_To_to_Repository()
        {
            var body = DateTime.Now.ToString();
            var id = Guid.NewGuid().ToString();

            _repository.Add(new Message
            {
                Body = body,
                Id = id,
                Type = _messageType
            });

            var msg = _repository.Pull(_messageType);
            Assert.That(msg.Body, Is.EqualTo(body));
            Assert.That(msg.Id, Is.EqualTo(id));
            Assert.That(msg.Type, Is.EqualTo(_messageType));
        }

        [TestCase(10)]
        [TestCase(33)]
        [TestCase(223)]
        [TestCase(88)]
        public void Add_Increments_Count(int expectedCount)
        {
            foreach (var message in Enumerable.Repeat(new Message {Type = _messageType}, expectedCount))
            {
                _repository.Add(message);
            }

            var actual = _repository.Count(_messageType);

            Assert.That(actual, Is.EqualTo(expectedCount));
        }

        [Test]
        public void NewMessage_Event_Triggered_With_CorrectType()
        {
            var actual = "";
            _repository.NewMessage+= (sender, s) => actual = s;

            _repository.Add(new Message {Type = _messageType});
            Thread.Sleep(500);
            Assert.That(actual, Is.EqualTo(_messageType));
        }
    }
}
