namespace DotQueue.Client
{
    internal interface IHttpAdapter<T>
    {
        string Add(T message);
        T Pull();
        int Count();
        bool Subscribe(int localPort);
    }
}