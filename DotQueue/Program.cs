using DotQueue.HostLib;
using Topshelf;
using Host = DotQueue.HostLib.Host;

namespace DotQueue
{
    class Program
    {
        private static int _apiPort = 8083;

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Host>(s =>
                {
                    s.ConstructUsing(factory => new Host(_apiPort));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DotQueue Host");
                x.SetDisplayName("DotQueue");
                x.SetServiceName("DotQueue");
            });
        }
    }
}