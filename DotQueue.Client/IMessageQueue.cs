namespace DotQueue.Client
{
    public interface IMessageQueue<T>
    {
        string Add(T message);
        T Pull();
    }
}