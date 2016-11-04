using System;

namespace DotQueue.Client
{
    internal interface IWaitDurationHelper
    {
        TimeSpan SubscribtionRenewalSpan();
    }

    internal class WaitDurationHelper : IWaitDurationHelper
    {
        public TimeSpan SubscribtionRenewalSpan()
        {
            return TimeSpan.FromMinutes(1);
        }
    }
}