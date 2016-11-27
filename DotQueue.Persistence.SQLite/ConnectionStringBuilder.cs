using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class ConnectionStringBuilder : IConnectionStringBuilder
    {
        public string Build(string dataFilePath)
        {
            return $"Data Source={dataFilePath};Version=3;";
        }
    }
}