using System.Collections.Generic;
using System.Linq;
using DotQueue.HostLib;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    /// <summary>
    /// SQLite persistence adapter for DotQueue host
    /// </summary>
    public class SQLitePersistenceAdapter : IPersistenceAdapter
    {
        private readonly ISqlAdapter _adapter;
        private readonly ICommandQueue _commandQueue;
        private readonly ICommandBuilder _commandBuilder;

        #region fields
        public SQLitePersistenceAdapter()
        {
            //need to look into introducing an IoC and get rid of manual constrution
            _commandBuilder = new CommandBuilder();
            var dataFileHelper = new DataFileHelper(new ConnectionStringBuilder());
            dataFileHelper.CreateDataFileAndTable();
            var queryExecutor = new QueryExecutor(dataFileHelper.GetDataFilePath());
            _adapter = new SqlAdapter(_commandBuilder, new MessagesDataReader(queryExecutor));
            _commandQueue = new CommandQueue(queryExecutor);
        }

        internal SQLitePersistenceAdapter(ISqlAdapter adapter, ICommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
            _adapter = adapter;
        }
        #endregion

        public IEnumerable<Message> GetAll()
        {
            var messages = _adapter.GetAll().Select(m => (Message)m);
            return messages;
        }

        public void Add(Message message)
        {
            _commandQueue.Enqueue(_commandBuilder.BuildAddCommand(message));
        }

        public void Delete(Message message)
        {
            _commandQueue.Enqueue(_commandBuilder.BuildDeleteCommand(message.Id));
        }
    }
}
