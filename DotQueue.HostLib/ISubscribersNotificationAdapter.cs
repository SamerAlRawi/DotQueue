namespace DotQueue.HostLib
{
    internal interface ISubscribersNotificationAdapter
    {
        void Notify(Subscriber client, string message);
    }
}