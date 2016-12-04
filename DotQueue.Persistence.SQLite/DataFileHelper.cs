using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class DataFileHelper : IDataFileHelper
    {
        private readonly Lazy<string> _filePath;
        private readonly IConnectionStringBuilder _connectionStringBuilder;

        public DataFileHelper(IConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
            _filePath = new Lazy<string>(GetFilePath);
        }

        public string GetDataFilePath()
        {
            return _filePath.Value;
        }

        public void CreateDataFileAndTable()
        {
            if (!File.Exists(_filePath.Value))
            {
                SQLiteConnection.CreateFile(_filePath.Value);
                var sql = "create table messages (table_id INTEGER primary key NOT NULL, id varchar(50), type varchar(150), body text);";
                using (var conn = new SQLiteConnection(_connectionStringBuilder.Build(_filePath.Value)))
                {
                    using (var cmd = new SQLiteCommand { CommandText = sql, CommandType = CommandType.Text })
                    {
                        cmd.Connection = conn;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private string GetFilePath()
        {
            var assemblyLocation = typeof(DataFileHelper).Assembly.Location;
            FileInfo info = new FileInfo(assemblyLocation);
            return Path.Combine(info.DirectoryName, Constants.DATABASE_FILENAME);
        }
    }
}