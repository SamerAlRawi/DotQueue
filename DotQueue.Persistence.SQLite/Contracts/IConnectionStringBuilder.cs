namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface IConnectionStringBuilder
    {
        string Build(string dataFilePath);
    }
}
