using System;
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

        [Test]
        public void Add_Increment_Count()
        {
            _repository.Add(new Message {Type = _messageType});
            _repository.Add(new Message {Type = _messageType});
            _repository.Add(new Message {Type = _messageType});

            Assert.That(_repository.Count(_messageType), Is.EqualTo(3));
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
