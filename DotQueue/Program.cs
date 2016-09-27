using System;
using Topshelf;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace DotQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Host>(s =>
                {
                    s.ConstructUsing(name => new Host());
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

    public class Host
    {
        public Host()
        {
            var config = new HttpSelfHostConfiguration("http://0.0.0.0:8080");

            config.Routes.MapHttpRoute("Default", "api/{controller}/{method}/{id}",
                new { id = RouteParameter.Optional });

            using (HttpSelfHostServer server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Press Enter to quit.");
                Console.ReadLine();
            }
        }
        public void Start() { Console.WriteLine("Host thread started."); }
        public void Stop() { Console.WriteLine("Host thread stopped"); }
    }
}
