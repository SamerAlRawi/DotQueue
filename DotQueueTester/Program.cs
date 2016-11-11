using System;
using System.Net;
using System.Threading;
using DotQueue.Client;
using DotQueue.HostLib;

namespace DotQueueTester
{
    class Program
    {
        private static int _queuePort = 8083;

        static void Main(string[] args)
        {
            StartQueueServer();
            Thread.Sleep(2000);//wait for port to start
            StartListening();
            Thread.Sleep(2000);

            var sender = new MessageQueue<MyMessage>(new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"), Port = _queuePort
            });

            while (true)
            {
                Console.WriteLine("Press ENTER to send new message");
                Console.ReadLine();
                sender.Add(new MyMessage());
            }
        }

        private static void StartListening()
        {
            var client = new MessageQueue<MyMessage>(new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = _queuePort
            });
            Run(() =>
            {
                foreach (var myMessage in client)
                {
                    Console.WriteLine("Message received:");
                    Console.WriteLine($"{myMessage.Name}, {myMessage.DateCreated}");
                }
            });
        }

        private static void StartQueueServer()
        {
            var queue = new QueueHost(_queuePort);
            Run(() => queue.Start());
        }

        private static void Run(Action a)
        {
            new Thread(() => a()).Start();
        }
    }

    class MyMessage
    {
        public string Name { get; set; } = "user1";
        public string Email { get; set; } = "my@email.com";
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
