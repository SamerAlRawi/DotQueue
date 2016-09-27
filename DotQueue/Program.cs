using System;
using System.Reflection;
using Topshelf;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microsoft.Owin.StaticFiles;
using Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles.ContentTypes;

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
        private HttpSelfHostServer _httpSelfHostServer;
        private IDisposable _staticFileHost;

        public void Start()
        {
            StartApiHost();
            StartFileHost();
        }

        private void StartFileHost()
        {
            var url = "http://*:8082/";
            var fileSystem = new PhysicalFileSystem("");
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem
            };
            options.StaticFileOptions.ContentTypeProvider = new CustomContentTypeProvider();
            _staticFileHost = WebApp.Start(url, builder => builder.UseFileServer(options));
        }

        private void StartApiHost()
        {
            HttpSelfHostConfiguration _configuration = new HttpSelfHostConfiguration("http://0.0.0.0:8080");
            _configuration.Routes.MapHttpRoute("ApiDefault", "api/{controller}/{method}/{id}",
                new { id = RouteParameter.Optional });
            _httpSelfHostServer = new HttpSelfHostServer(_configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _httpSelfHostServer.CloseAsync();
            _httpSelfHostServer.Dispose();

            _staticFileHost.Dispose();
        }
    }
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            //Mappings.Add(".js", "text/javascript");
        }
    }
}