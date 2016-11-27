using System.Data.SQLite;

namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface ICommandQueue
    {
        void Enqueue(SQLiteCommand command);
    }
}