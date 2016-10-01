using System;
using System.Net;

namespace DotQueue.Client.DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Read();
            while (true)
            {
                var queue = new MessageQueue<M>(new DotQueueAddress
                {
                    IpAddress = IPAddress.Parse("127.0.0.1"), Port = 8083
                });
                queue.Add(new M { BlaBla = "bla bls" });
            }
        }
    }

    public class M
    {
        public string BlaBla { get; set; }
    }
}
