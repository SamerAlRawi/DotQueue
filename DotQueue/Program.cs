using System;
using DotQueue.HostLib;
using Topshelf;

namespace DotQueue
{
    class Program
    {
        private static int _apiPort = 8083;

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<QueueHost>(s =>
                {
                    s.ConstructUsing(factory => new QueueHost(_apiPort));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DotQueue QueueHost");
                x.SetDisplayName("DotQueue");
                x.SetServiceName("DotQueue");
            });
            Console.Read();
        }
    }
}