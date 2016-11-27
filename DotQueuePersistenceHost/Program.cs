using System;
using DotQueue.HostLib;
using DotQueue.Persistence.SQLite;
using Raven.Client;
using Raven.Client.Document;

namespace DotQueuePersistenceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpPort = 8083; //Can be any other port#

            #region RavenDb example
            /*
            IDocumentStore address = new DocumentStore {
                Url = "http://localhost:8080",
                DefaultDatabase = "Customers",
            };
            var host = new QueueHost(httpPort, persistenceAdapter:new RavenDbPersistenceAdapter(address));
            */
            #endregion

            #region Sqlite example
            var host = new QueueHost(httpPort, persistenceAdapter: new SQLitePersistenceAdapter());
            #endregion

            host.Start();
            Console.ReadLine();

        }
    }
}
