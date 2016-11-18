using System;
using DotQueue.HostLib;
using DotQueue.Persistence.RavenDB;
using Raven.Client;
using Raven.Client.Document;

namespace DotQueuePersistenceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpPort = 8083; //Can be any other port#
            IDocumentStore address = new DocumentStore {
                Url = "http://localhost:8080",
                DefaultDatabase = "Customers",
            };
            var host = new QueueHost(httpPort, persistenceAdapter:new RavenDbPersistenceAdapter(address));
            host.Start();
            Console.ReadLine();

        }
    }
}
