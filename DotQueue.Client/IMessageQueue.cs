using System.Collections.Generic;

namespace DotQueue.Client
{
    public interface IMessageQueue<T> : IEnumerable<T>
    {
        string Add(T message);
        T Pull();
        int Count();
    }
}