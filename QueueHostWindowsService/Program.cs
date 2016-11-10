using DotQueue.HostLib;
using Topshelf;

namespace QueueHostWindowsService
{
    class Program
    {
        private static int _apiPort = 8083;

        static void Main(string[] args)
        {
            //to install windows service open cmd.exe as an administrator and cd to project build folder
            //run the following command 
            //QueueHostWindowsService.exe install
            HostFactory.Run(x =>
            {
                x.Service<QueueHost>(s =>
                {
                    s.ConstructUsing(factory => new QueueHost(_apiPort));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("DotQueue QueueHost");
                x.SetDisplayName("DotQueue");
                x.SetServiceName("DotQueue");
            });
        }
    }
}
