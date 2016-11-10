using System;
using System.Net;
using System.Threading;
using DotQueue.Client;
using SharedLib;

namespace DotQueueClientSender
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1500);//wait for queue listener to open port
            //create a queue address configuration
            var queueAddress = new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = 8083
            };
            //initialize queue client
            var client = new MessageQueue<UserProfile>(queueAddress);
            Console.WriteLine("Press CTRL+C to exit");
            //loop and send messages
            while (true)
            {
                var uniqueMessageId = client.Add(new UserProfile
                {
                    Name = "user1",
                    Address = "1001 south rd.",
                    LastLogin = DateTime.Now
                });
                Console.WriteLine($"message delivered:{uniqueMessageId}");
            }
        }
    }
}
