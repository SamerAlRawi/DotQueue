using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using DotQueue.Persistence.SQLite.Contracts;

namespace DotQueue.Persistence.SQLite
{
    internal class CommandQueue : ConcurrentQueue<SQLiteCommand>, ICommandQueue
    {
        private IQueryExecutor _queryExecutor;

        public CommandQueue(IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
            StartProcessingThread();
        }

        private void StartProcessingThread()
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (Count > 0)
                    {
                        var commands = new List<SQLiteCommand>();
                        while (Count>0)
                        {
                            foreach (var cmd in this)//TODO some other way to enumerate
                            {
                                SQLiteCommand sqLiteCommand = null;
                                TryDequeue(out sqLiteCommand);
                                commands.Add(sqLiteCommand);
                            }
                        }
                        _queryExecutor.ExecuteNonQuery(commands);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }).Start();
        }
    }
}