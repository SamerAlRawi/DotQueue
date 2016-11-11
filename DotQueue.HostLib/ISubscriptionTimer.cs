using System;

namespace DotQueue.HostLib
{
    internal interface ISubscriptionTimer
    {
        TimeSpan RenewalInterval();
    }
}