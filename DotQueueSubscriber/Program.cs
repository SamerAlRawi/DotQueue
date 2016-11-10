using System;
using System.Net;
using System.Threading;
using DotQueue.Client;
using SharedLib;

namespace DotQueueSubscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(3000);//wait for queue listener to open port
            //create a queue address configuration
            var queueAddress = new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = 8083
            };
            //initialize queue client
            var messageQueue = new MessageQueue<UserProfile>(queueAddress);
            //subscribe, loop forever and process messages
            foreach (var userProfile in messageQueue)
            {
                Console.WriteLine($"{userProfile.Name}, LastLogin:{userProfile.LastLogin}");
            }
        }
    }
}
