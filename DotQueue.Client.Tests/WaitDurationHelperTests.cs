using NUnit.Framework;

namespace DotQueue.Client.Tests
{
    [TestFixture]
    public class WaitDurationHelperTests
    {
        [Test]
        public void Timespans_MakingSense()
        {
            var helper = new WaitDurationHelper();

            Assert.That(helper.SubscribtionRenewalSpan().Seconds, Is.EqualTo(30));
            Assert.That(helper.NewMessageWaitDuration().Milliseconds, Is.EqualTo(100));
        }
    }
}
