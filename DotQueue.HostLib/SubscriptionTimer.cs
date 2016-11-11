using System;

namespace DotQueue.HostLib
{
    internal class SubscriptionTimer : ISubscriptionTimer
    {
        public TimeSpan RenewalInterval()
        {
            return TimeSpan.FromSeconds(10);
        }
    }
}