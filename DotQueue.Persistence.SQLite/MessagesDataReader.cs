using System.Collections.Generic;
using System.Data.SQLite;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class MessagesDataReader : IMessagesDataReader
    {
        private readonly IQueryExecutor _queryExecutor;

        public MessagesDataReader(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        public IEnumerable<PersistedMessage> ExecuteAndReadResults(SQLiteCommand command)
        {
            var reader = _queryExecutor.ExecuteReader(command);
            var result = new List<PersistedMessage>();
            while (reader.Read())
            {
                result.Add(new PersistedMessage()
                {
                    TableId = reader.GetInt64(0),
                    Id = reader.GetString(1),
                    Type = reader.GetString(2),
                    Body = reader.GetString(3)
                });
            }
            reader.Close();
            return result;
        }
    }
}