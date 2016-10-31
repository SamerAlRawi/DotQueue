using System;
using System.Net;
using System.Threading;

namespace DotQueue.Client.DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Read();
            var queue = new MessageQueue<Subscriber>(new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = 8083
            });
            Action a = () =>
            {
                while (true)
                {
                    var messageId = queue.Add(new Subscriber());
                    Console.WriteLine(messageId);
                    Thread.Sleep(100);
                    Console.WriteLine($"Count: {queue.Count()}");
                    Console.WriteLine(queue.Pull());
                }
            };
            a();

            //for (int i = 0; i < 10; i++)
            //{
            //    Thread t1 = new Thread(new ThreadStart(a));
            //    t1.Start();
            //}
            Console.Read();
        }
    }

    public class Subscriber
    {
        public string Id { get; set; } = "1001";
        public string Name { get; set; } = "User1";
        public string Email { get; set; } = "user@email.tld";
        public int ZipCode { get; set; } = 44101;
    }
}
