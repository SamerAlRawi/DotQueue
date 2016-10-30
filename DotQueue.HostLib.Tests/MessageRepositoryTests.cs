using System;
using DotQueue.HostLib;
using NUnit.Framework;

namespace DotQueue.Repository.Tests
{
    [TestFixture]
    public class MessageRepositoryTests
    {
        MessageRepository _repository = new MessageRepository();

        [Test]
        public void Add_Adds_Message_To_to_Repository()
        {
            var body = DateTime.Now.ToString();
            var id = Guid.NewGuid().ToString();
            var messageType = typeof(DateTime).ToString();

            _repository.Add(new Message
            {
                Body = body,
                Id = id,
                Type = messageType
            });

            var msg = _repository.Pull(messageType);
            Assert.That(msg.Body, Is.EqualTo(body));
            Assert.That(msg.Id, Is.EqualTo(id));
            Assert.That(msg.Type, Is.EqualTo(messageType));
        }
    }
}
