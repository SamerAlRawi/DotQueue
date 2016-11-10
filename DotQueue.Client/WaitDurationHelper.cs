using System;

namespace DotQueue.Client
{
    internal class WaitDurationHelper : IWaitDurationHelper
    {
        public TimeSpan SubscribtionRenewalSpan()
        {
            return TimeSpan.FromSeconds(30);
        }

        public TimeSpan NewMessageWaitDuration()
        {
            return TimeSpan.FromMilliseconds(100);
        }
    }
}