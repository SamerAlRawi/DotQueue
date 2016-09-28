using DotQueue.HostLib;
using Topshelf;
using Host = DotQueue.HostLib.Host;

namespace DotQueue
{
    class Program
    {
        private static int _apiPort = 8083;
        private static int _dashboardPort = 8082;

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Host>(s =>
                {
                    s.ConstructUsing(name =>
                    {
                        var hostParameters = new HostParameters { ApiPort = _apiPort, DashboardPort = _dashboardPort };
                        return new Host(hostParameters);
                    });
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