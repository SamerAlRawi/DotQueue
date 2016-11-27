using System.Data.SQLite;
using DotQueue.HostLib;

namespace DotQueue.Persistence.SQLite.Contracts
{
    internal interface ICommandBuilder
    {
        SQLiteCommand BuildAddCommand(Message message);
        SQLiteCommand BuildDeleteCommand(string id);
        SQLiteCommand BuildGetAll();
    }
}