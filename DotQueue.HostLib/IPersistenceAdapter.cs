using System.Collections.Generic;

namespace DotQueue.HostLib
{
    public interface IPersistenceAdapter
    {
        IEnumerable<Message> GetAll();
        void Add(Message customer);
        void Delete(Message customer);
    }
}