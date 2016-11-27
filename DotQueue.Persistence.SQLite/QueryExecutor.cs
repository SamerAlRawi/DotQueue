using System.Collections.Generic;
using System.Data.SQLite;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class QueryExecutor : IQueryExecutor
    {
        private readonly string _databaseFileName;
        private readonly SQLiteConnection _dbConnection;

        public QueryExecutor(string dataFilePath)
        {
            _databaseFileName = dataFilePath;
            _dbConnection = new SQLiteConnection($"Data Source={_databaseFileName};Version=3;");
            _dbConnection.Open();
        }

        public void ExecuteNonQuery(IEnumerable<SQLiteCommand> command)
        {
            using (var transaction = _dbConnection.BeginTransaction())
            {
                foreach (var sqLiteCommand in command)
                {
                    sqLiteCommand.Connection = _dbConnection;
                    sqLiteCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        public SQLiteDataReader ExecuteReader(SQLiteCommand command)
        {
            command.Connection = _dbConnection;
            return command.ExecuteReader();
        }
    }
}