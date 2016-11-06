using System;

namespace DotQueue.Client
{
    internal class WaitDurationHelper : IWaitDurationHelper
    {
        public TimeSpan SubscribtionRenewalSpan()
        {
            return TimeSpan.FromMinutes(1);
        }

        public TimeSpan NewMessageWaitDuration()
        {
            return TimeSpan.FromMilliseconds(100);
        }
    }
}