using System;

namespace DotQueue.Client
{
    internal interface IListenerAdapter<T>
    {
        void StartListener(int port);
        event EventHandler<QueueNotification> NotificationReceived;
    }
}