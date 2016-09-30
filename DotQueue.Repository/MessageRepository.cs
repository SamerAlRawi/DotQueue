using System.Collections.Concurrent;
using DotQueue.Domain;

namespace DotQueue.Repository
{
    public class MessageRepository : IMessageRepository
    {
        ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();

        public void Add(Message message)
        {
            _messages.Enqueue(message);
        }

        public Message Pull()
        {
            Message message = null;
            _messages.TryDequeue(out message);
            return message;
        }
    }
}
