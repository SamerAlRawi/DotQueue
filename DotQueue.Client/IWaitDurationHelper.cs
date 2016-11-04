using System;

namespace DotQueue.Client
{
    internal interface IWaitDurationHelper
    {
        TimeSpan SubscribtionRenewalSpan();
        TimeSpan NewMessageWaitDuration();
    }

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