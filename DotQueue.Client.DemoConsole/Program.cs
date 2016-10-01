using System;
using System.Globalization;
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
            var queue = new MessageQueue<M>(new DotQueueAddress
            {
                IpAddress = IPAddress.Parse("127.0.0.1"),
                Port = 8083
            });
            Action a = () =>
            {
                while (true)
                {
                    var messageId = queue.Add(new M { BlaBla = "bla bls" });
                    Console.WriteLine(messageId);
                }
            };

            for (int i = 0; i < 10; i++)
            {
                Thread t1 = new Thread(new ThreadStart(a));
                t1.Start();
            }
            Console.Read();
        }
    }

    public class M
    {
        public string BlaBla { get; set; }
    }
}
