using System.Collections.Generic;
using System.Data.SQLite;

namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface IQueryExecutor
    {
        void ExecuteNonQuery(IEnumerable<SQLiteCommand> command);
        SQLiteDataReader ExecuteReader(SQLiteCommand command);
    }
}