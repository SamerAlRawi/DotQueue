using System;
using DotQueue.HostLib;

namespace QueueHostConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpPort = 8083;
            var queue = new QueueHost(httpPort);
            queue.Start();
            Console.WriteLine($"DotQueue listener is listening on port {httpPort}");
            Console.ReadLine();
        }
    }
}
