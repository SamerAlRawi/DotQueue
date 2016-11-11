namespace DotQueue.HostLib
{
    public interface ISubscribersNotificationAdapter
    {
        void Notify(Subscriber client, string message);
    }
}