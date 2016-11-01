using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    var messageId = queue.Add(new Subscriber());
                    Console.WriteLine(messageId);
                }
            });
            Thread.Sleep(1000);//wait for the subscription
            foreach (var subscriber in queue)
            {
                Console.WriteLine(subscriber.Email);
            }
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
