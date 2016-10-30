namespace DotQueue.HostLib
{
    public interface IMessageRepository
    {
        void Add(Message message);
        Message Pull(string messageType);
        int Count(string messageType);
    }
}