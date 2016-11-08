using System.Threading;
using DotQueue.HostLib;
using NSubstitute;
using NUnit.Framework;

namespace DotQueue.HostLib.Tests
{
    [TestFixture]
    public class QueueControllerTests
    {
        private QueueController _controller;
        private IMessageRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = Substitute.For<IMessageRepository>();
            _controller = new QueueController(_repository);    
        }

        [Test]
        public void Add_Adds_Message_To_Repository()
        {
            var message = new Message();
            _controller.Add(message);
            Thread.Sleep(1000);
            _repository.Received().Add(message);
        }

        [Test]
        public void Pull_Return_Message_Using_Category()
        {
            var category = "MessageType1";
            var message = new Message();
            _repository.Pull(category).Returns(message);

            var actual = _controller.Pull(category);

            Assert.AreEqual(actual, message);
        }

        [Test]
        public void Count_Returns_Message_Count_Using_Category()
        {
            var category = "MessageType1";
            var count = 2343;
            _repository.Count(category).Returns(count);

            var actual = _controller.Count(category);

            Assert.AreEqual(actual, count);
        }
    }
}
