using System.Collections.Concurrent;

namespace DotQueue.HostLib
{
    public class MessageRepository : IMessageRepository
    {
        ConcurrentDictionary<string, ConcurrentQueue<Message>> _dictionary = new ConcurrentDictionary<string, ConcurrentQueue<Message>>();
        
        public void Add(Message message)
        {
            CheckAndAddCitonary(message.Type);
            _dictionary[message.Type].Enqueue(message);
        }

        private void CheckAndAddCitonary(string messageType)
        {
            if (!_dictionary.ContainsKey(messageType))
            {
                _dictionary.TryAdd(messageType, new ConcurrentQueue<Message>());
            }
        }

        public Message Pull(string messageType)
        {
            Message message = null;
            if (_dictionary.ContainsKey(messageType))
            {
                _dictionary[messageType].TryDequeue(out message);
            }
            return message;
        }

        public int Count(string messageType)
        {
            if (_dictionary.ContainsKey(messageType))
            {
                return _dictionary[messageType].Count;
            }
            return 0;
        }
    }
}
