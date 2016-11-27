using System.Data.SQLite;
using DotQueue.HostLib;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class CommandBuilder : ICommandBuilder
    {
        public SQLiteCommand BuildAddCommand(Message message)
        {
            return new SQLiteCommand($"insert into messages(id, type, body) values('{message.Id}', '{message.Type}', '{message.Body}')");
        }

        public SQLiteCommand BuildDeleteCommand(string id)
        {
            return new SQLiteCommand($"delete from messages where id='{id}'");
        }

        public SQLiteCommand BuildGetAll()
        {
            return new SQLiteCommand("select id,type,body from messages");
        }
    }
}