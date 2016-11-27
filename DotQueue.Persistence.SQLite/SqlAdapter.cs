using System.Collections.Generic;
using System.Linq;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class SqlAdapter : ISqlAdapter
    {
        private readonly IMessagesDataReader _reader;
        private readonly ICommandBuilder _commandBuilder;

        public SqlAdapter(ICommandBuilder commandBuilder, IMessagesDataReader reader)
        {
            _commandBuilder = commandBuilder;
            _reader = reader;
        }

        public IEnumerable<PersistedMessage> GetAll()
        {
            return _reader.ExecuteAndReadResults(_commandBuilder.BuildGetAll()).OrderBy(m => m.TableId);
        }
    }
}