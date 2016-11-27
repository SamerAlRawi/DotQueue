using System.Collections.Generic;

namespace DotQueue.HostLib
{
    public interface IPersistenceAdapter
    {
        IEnumerable<Message> GetAll();
        void Add(Message message);
        void Delete(Message customer);
    }
}