using DotQueue.Domain;

namespace DotQueue.Repository
{
    public interface IMessageRepository
    {
        void Add(Message message);
        Message Pull(string messageType);
    }
}