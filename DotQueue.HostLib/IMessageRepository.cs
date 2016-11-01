using System;

namespace DotQueue.HostLib
{
    public interface IMessageRepository
    {
        void Add(Message message);
        event EventHandler<string> NewMessage;
        Message Pull(string messageType);
        int Count(string messageType);
    }
}