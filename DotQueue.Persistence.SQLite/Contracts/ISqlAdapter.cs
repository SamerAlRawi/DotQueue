using System.Collections.Generic;

namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface ISqlAdapter
    {
        IEnumerable<PersistedMessage> GetAll();
    }
}