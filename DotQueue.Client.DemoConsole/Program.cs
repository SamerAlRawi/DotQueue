using System;
using System.Linq;
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
                    var uniqueMessaeId = queue.Add(new Subscriber());
                    Thread.Sleep(500);
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
        public string Email { get; set; } = $"{Guid.NewGuid().ToString().Split('-').First()}@gmail.com";
        public int ZipCode { get; set; } = 44101;
    }
}
