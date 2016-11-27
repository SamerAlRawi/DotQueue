namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface IDataFileHelper
    {
        string GetDataFilePath();
        void CreateDataFileAndTable();
    }
}