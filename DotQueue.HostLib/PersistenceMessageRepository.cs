using System.Collections.Generic;

namespace DotQueue.HostLib
{
    /// <summary>
    /// Persistance message repository
    /// </summary>
    internal class PersistenceMessageRepository : MessageRepository, IMessageRepository
    {
        private readonly IPersistenceAdapter _persistenceAdapter;

        public PersistenceMessageRepository(IPersistenceAdapter persistenceAdapter)
        {
            _persistenceAdapter = persistenceAdapter;
            var persistedItems = _persistenceAdapter.GetAll();
            AddMessageInMemory(persistedItems);
        }

        private void AddMessageInMemory(IEnumerable<Message> persistedItems)
        {
            foreach (var persistedItem in persistedItems)
            {
                base.Add(persistedItem);
            }
        }

        public void Add(Message message)
        {
            _persistenceAdapter.Add(message);
            base.Add(message);
        }

        public Message Pull(string messageType)
        {
            var message = base.Pull(messageType);
            _persistenceAdapter.Delete(message);
            return message;
        }
    }
}