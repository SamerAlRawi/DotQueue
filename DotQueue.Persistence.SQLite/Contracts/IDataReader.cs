using System.Collections.Generic;
using System.Data.SQLite;

namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface IMessagesDataReader
    {
        IEnumerable<PersistedMessage> ExecuteAndReadResults(SQLiteCommand command);
    }
}