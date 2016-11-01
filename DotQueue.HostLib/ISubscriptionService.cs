namespace DotQueue.HostLib
{
    public interface ISubscriptionService
    {
        void Subscribe(string clientAddress, int port, string category);
    }
}