using System;

namespace DotQueue.Client
{
    internal interface IWaitDurationHelper
    {
        TimeSpan SubscribtionRenewalSpan();
        TimeSpan NewMessageWaitDuration();
    }
}